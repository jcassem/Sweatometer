using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sweatometer.Model;
using Sweatometer.Service;

namespace Sweatometer.Data.Emoji {

    public class EmojiDataGenerator : IEmojiDataGenerator {

        private static readonly int TOP_RESULTS_AMOUNT = 10;

        private readonly IWordFinderService wordFinderService;

        private readonly IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions;

        public EmojiDataGenerator(
            IWordFinderService wordFinderService,
            IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions
        ) {
            this.wordFinderService = wordFinderService;
            this.emojiRelatedWordOptions = emojiRelatedWordOptions;
        }

        public async Task CreateRelatedWordsDictionary() {
            await GenerateRelatedWordsDictionary();
            Console.WriteLine("Writing related words to file.");
            PersistRelatedWordDictionaryToJsonFile(EmojiDataLoader.EMOJI_RELATED_WORDS_JSON_FILE_PATH);
        }

        private async Task GenerateRelatedWordsDictionary() {
            var initialKeys = new List<string>(EmojiData.EmojiDictionary.Keys);
            int sum = initialKeys.Count;
            int count = 0;

            bool includeSynonyms = emojiRelatedWordOptions?.Value?.MaxAmountSynonyms > 0 == true;
            bool includeRelatedWords = emojiRelatedWordOptions?.Value?.MaxAmountRelatedWords > 0 == true;

            foreach (var key in initialKeys) {
                var relatedWords = new List<SimilarWord>();

                if (includeSynonyms) {
                    var foundWords = await FindTopSynonymsFor(key);
                    relatedWords.AddRange(foundWords);
                }

                if (includeRelatedWords) {
                    var foundWords = await FindTopRelatedWordsFor(key);
                    relatedWords.AddRange(foundWords);
                }

                if (relatedWords.Any()) {
                    AddToRelatedWordsDictionary(key, relatedWords.OrderByDescending(x => x.Score).ToList());
                }

                count++;
                Console.WriteLine($"{count}/{sum} related words processed.");
            }
        }

        private async Task<List<SimilarWord>> FindTopSynonymsFor(string searchTerm) {
            int minScore = emojiRelatedWordOptions?.Value?.MinScoreForSynoymns ?? 0;
            var foundWords = await wordFinderService.GetWordsToMeanLikeAsync(searchTerm);

            var topResults = foundWords
                .Where(x => x.Score >= minScore)
                .OrderByDescending(s => s.Score)
                .Take(emojiRelatedWordOptions?.Value?.MaxAmountSynonyms ?? TOP_RESULTS_AMOUNT)
                .ToList();

            foreach (var relatedWord in topResults) {
                relatedWord.Type = SimilarWordType.MEANS_LIKE;
                relatedWord.Score = (relatedWord.Score*100)/minScore;
            }

            return topResults;
        }

        private async Task<List<SimilarWord>> FindTopRelatedWordsFor(string searchTerm) {
            int minScore = emojiRelatedWordOptions?.Value?.MinScoreForRelatedWords ?? 0;
            var foundWords = await wordFinderService.GetRelatedTriggerWords(searchTerm);

            var topResults = foundWords
                .Where(x => x.Score >= minScore)
                .OrderByDescending(s => s.Score)
                .Take(emojiRelatedWordOptions?.Value?.MaxAmountRelatedWords ?? TOP_RESULTS_AMOUNT)
                .ToList();

            foreach (var relatedWord in topResults) {
                relatedWord.Type = SimilarWordType.RELATED;
                relatedWord.Score = (relatedWord.Score*100)/minScore;
            }

            return topResults;
        }

        private static void AddToRelatedWordsDictionary(string key, List<SimilarWord> relatedWords) {
            if (EmojiData.RelatedWordsDictionary.ContainsKey(key)) {
                var currentWords = EmojiData.RelatedWordsDictionary[key].ToList();

                currentWords.AddRange(relatedWords.Except(currentWords));
                EmojiData.relatedWordsDictionary[key] = currentWords;
            }
            else {
                EmojiData.relatedWordsDictionary.TryAdd(key, relatedWords);
            }
        }


        private void PersistRelatedWordDictionaryToJsonFile(string filePath) {
            string json = JsonConvert.SerializeObject(EmojiData.RelatedWordsDictionary, Formatting.Indented);
            Console.WriteLine("Saving json (of length " + json.Length + ") to: " + filePath);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Saved");
        }
    }
}