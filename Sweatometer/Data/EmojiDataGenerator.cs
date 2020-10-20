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

    /// <inheritdoc/>
    public class EmojiDataGenerator : IEmojiDataGenerator {

        /// <summary>
        /// Fallback on number of related words to consider if its not set in config.
        /// </summary>
        private static readonly int NUMBER_OF_RELATED_WORDS_TO_SAVE = 10;

        /// <summary>
        /// Word finder service to find related words.
        /// </summary>
        private readonly IWordFinderService wordFinderService;

        /// <summary>
        /// Config options to define what to search for and what to save.
        /// </summary>
        private readonly IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="wordFinderService"></param>
        /// <param name="emojiRelatedWordOptions"></param>
        public EmojiDataGenerator(
            IWordFinderService wordFinderService,
            IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions
        ) {
            this.wordFinderService = wordFinderService;
            this.emojiRelatedWordOptions = emojiRelatedWordOptions;
        }

        ///<inheritdoc />
        public async Task GenerateRelatedWordsDictionaryFile() {
            Console.WriteLine("Geneating word dictionary.");
            await GenerateRelatedWordsDictionary();

            Console.WriteLine("Writing related words to file.");
            PersistRelatedWordDictionaryToJsonFile(EmojiDataLoader.EMOJI_RELATED_WORDS_JSON_FILE_PATH);
        }

        /// <summary>
        /// Generates the related word dictionary.
        /// </summary>
        /// <returns>Task.</returns>
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

        /// <summary>
        /// Finds synonyms for <param name="searchTerm"/> limiting the results based on the criteria set in config.
        /// All scores are normalised against the minimum accepted score.
        /// </summary>
        /// <param name="searchTerm">Term to search for.</param>
        /// <returns>Top results related to search term.</returns>
        private async Task<List<SimilarWord>> FindTopSynonymsFor(string searchTerm) {
            int minScore = emojiRelatedWordOptions?.Value?.MinScoreForSynoymns ?? 1;
            var foundWords = await wordFinderService.GetWordsToMeanLikeAsync(searchTerm);

            var topResults = foundWords
                .Where(x => x.Score >= minScore)
                .OrderByDescending(s => s.Score)
                .Take(emojiRelatedWordOptions?.Value?.MaxAmountSynonyms ?? NUMBER_OF_RELATED_WORDS_TO_SAVE)
                .ToList();

            foreach (var relatedWord in topResults) {
                relatedWord.Type = SimilarWordType.MEANS_LIKE;
                relatedWord.Score = (relatedWord.Score * 100) / minScore;
            }

            return topResults;
        }

        /// <summary>
        /// Finds related words for <param name="searchTerm"/> limiting the results based on the criteria set in config.
        /// All scores are normalised against the minimum accepted score.
        /// </summary>
        /// <param name="searchTerm">Term to search for.</param>
        /// <returns>Top results related to search term.</returns>
        private async Task<List<SimilarWord>> FindTopRelatedWordsFor(string searchTerm) {
            int minScore = emojiRelatedWordOptions?.Value?.MinScoreForRelatedWords ?? 1;
            var foundWords = await wordFinderService.GetRelatedTriggerWords(searchTerm);

            var topResults = foundWords
                .Where(x => x.Score >= minScore)
                .OrderByDescending(s => s.Score)
                .Take(emojiRelatedWordOptions?.Value?.MaxAmountRelatedWords ?? NUMBER_OF_RELATED_WORDS_TO_SAVE)
                .ToList();

            foreach (var relatedWord in topResults) {
                relatedWord.Type = SimilarWordType.RELATED;
                relatedWord.Score = (relatedWord.Score * 100) / minScore;
            }

            return topResults;
        }

        /// <summary>
        /// Adds a related word to dictionary.
        /// </summary>
        /// <param name="key">Key to add against.</param>
        /// <param name="relatedWords">Reltaed words to add.</param>
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

        /// <summary>
        /// Saves the related word dictionary to a json file.
        /// </summary>
        /// <param name="filePath">File path to save against.</param>
        private void PersistRelatedWordDictionaryToJsonFile(string filePath) {
            string json = JsonConvert.SerializeObject(EmojiData.RelatedWordsDictionary, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Saved");
        }
    }
}