using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sweatometer.Model;
using Sweatometer.Service;

namespace Sweatometer {

    public class EmojiLoader : IEmojiLoader {
        private static readonly string EMOJI_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/emoji.json";
        private static readonly string EMOJI_RELATED_WORDS_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/emojiRelatedWords.json";

        private static readonly int TOP_RESULTS_AMOUNT = 10;

        private static ConcurrentDictionary<string, List<string>> emojiDictionary { get; set; }
        public static ConcurrentDictionary<string, List<string>> EmojiDictionary {
            get {
                return emojiDictionary;
            }
        }

        private static ConcurrentDictionary<string, List<SimilarWord>> relatedWordsForEmojiDictionary { get; set; }
        public static ConcurrentDictionary<string, List<string>> RelatedWordsForEmojiDictionary {
            get {
                return emojiDictionary;
            }
        }

        private readonly IWordFinderService wordFinderService;

        private readonly IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions;

        public EmojiLoader(
            IWordFinderService wordFinderService,
            IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions
        ) {
            this.wordFinderService = wordFinderService;
            this.emojiRelatedWordOptions = emojiRelatedWordOptions;
        }

        public void LoadEmojis() {
            LoadEmojisFromFile();
            LoadEmojiRelatedWordsFromFile();
        }

        private static void LoadEmojisFromFile() {
            try{
                var jsonRaw = File.ReadAllText(EMOJI_JSON_FILE_PATH);
                var values = JsonConvert.DeserializeObject<ConcurrentDictionary<string, List<string>>>(jsonRaw);
                if (values != null) {
                    emojiDictionary = values;
                }
            }
            catch(FileNotFoundException){
                // ignore
            }
            finally{
                if(emojiDictionary == null){
                    emojiDictionary = new ConcurrentDictionary<string, List<string>>();
                }
            }
        }

        private static void LoadEmojiRelatedWordsFromFile() {
            try{
                var jsonRaw = File.ReadAllText(EMOJI_RELATED_WORDS_JSON_FILE_PATH);
                var values = JsonConvert.DeserializeObject<ConcurrentDictionary<string, List<SimilarWord>>>(jsonRaw);
                if (values != null) {
                    relatedWordsForEmojiDictionary = values;
                }
            }
            catch(FileNotFoundException){
                // ignore
            }
            finally{
                if(relatedWordsForEmojiDictionary == null){
                    relatedWordsForEmojiDictionary = new ConcurrentDictionary<string, List<SimilarWord>>();
                }
            }
        }

        private void persistRelatedWordDictionaryToJsonFile(string filePath) {
            string json = JsonConvert.SerializeObject(relatedWordsForEmojiDictionary, Formatting.Indented);

            File.WriteAllText(filePath, json);
        }

        public async Task AddRelatedWordsToEmojiDictionary() {
            await CreateRelatedWordsDictionary();
            Console.WriteLine("Writing related words to file.");
            persistRelatedWordDictionaryToJsonFile(EMOJI_RELATED_WORDS_JSON_FILE_PATH);
        }

        public async Task CreateRelatedWordsDictionary() {
            var initialKeys = new List<string>(emojiDictionary.Keys);
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
                    AddToEmojiRelatedWords(key, relatedWords);
                }

                count++;
                Console.WriteLine($"{count}/{sum} related words processed.");
            }
        }

        private async Task<List<SimilarWord>> FindTopSynonymsFor(string searchTerm) {
            int minScoreForSynonyms = emojiRelatedWordOptions?.Value?.MinScoreForSynoymns ?? 0;
            var foundWords = await wordFinderService.GetWordsToMeanLikeAsync(searchTerm);

            var topResults = foundWords
                .Where(x => x.Score >= minScoreForSynonyms)
                .OrderByDescending(s => s.Score)
                .Take(emojiRelatedWordOptions?.Value?.MaxAmountSynonyms ?? TOP_RESULTS_AMOUNT)
                .ToList();

            foreach (var relatedWord in topResults) {
                relatedWord.Type = SimilarWordType.MEANS_LIKE;
            }

            return topResults;
        }

        private async Task<List<SimilarWord>> FindTopRelatedWordsFor(string searchTerm) {
            int minScoreForRetaltedWords = emojiRelatedWordOptions?.Value?.MinScoreForRelatedWords ?? 0;
            var foundWords = await wordFinderService.GetRelatedTriggerWords(searchTerm);

            var topResults = foundWords
                .Where(x => x.Score >= minScoreForRetaltedWords)
                .OrderByDescending(s => s.Score)
                .Take(emojiRelatedWordOptions?.Value?.MaxAmountRelatedWords ?? TOP_RESULTS_AMOUNT)
                .ToList();

            foreach (var relatedWord in topResults) {
                relatedWord.Type = SimilarWordType.RELATED;
            }

            return topResults;
        }

        private static void AddToEmojiRelatedWords(string key, List<SimilarWord> relatedWords) {
            if (relatedWordsForEmojiDictionary.ContainsKey(key)) {
                var currentWords = relatedWordsForEmojiDictionary[key].ToList();

                currentWords.AddRange(relatedWords.Except(currentWords));
                relatedWordsForEmojiDictionary.TryUpdate(key, currentWords, currentWords);
            }
            else {
                relatedWordsForEmojiDictionary.TryAdd(key, relatedWords);
            }
        }
    }
}