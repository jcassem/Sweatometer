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

        private static ConcurrentDictionary<string, List<SimilarWord>> relatedWordsDictionary { get; set; }


        private static ConcurrentDictionary<string, List<string>> relatedWordsToEmojiDictionaryKeysDictionary { get; set; }
        public static ConcurrentDictionary<string, List<string>> RelatedWordsToEmojiDictionaryKeysDictionary {
            get {
                return relatedWordsToEmojiDictionaryKeysDictionary;
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
            GenerateRelatedWordsToEmojiDictionaryKeysDictionary();
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
                // ignore for now
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
                    relatedWordsDictionary = values;
                }
            }
            catch(FileNotFoundException){
                // ignore for now
            }
            finally{
                if(relatedWordsDictionary == null){
                    relatedWordsDictionary = new ConcurrentDictionary<string, List<SimilarWord>>();
                }
            }
        }

        private static void GenerateRelatedWordsToEmojiDictionaryKeysDictionary() {
            if(RelatedWordsToEmojiDictionaryKeysDictionary == null){
                var relatedWordReverseLookup = new ConcurrentDictionary<string, List<string>>();

                foreach(var relatedWordKey in relatedWordsDictionary.Keys){
                    var relatedWords = relatedWordsDictionary[relatedWordKey];
                    foreach(var relatedWord in relatedWords){
                        var reverseWordKey = relatedWord.Word;
                        var reverseWordValue = relatedWordKey;

                        if(relatedWordReverseLookup.ContainsKey(reverseWordKey)){
                            // replace it if the Score is larger
                            var existingRelatedWords = relatedWordReverseLookup[reverseWordKey];
                            if(existingRelatedWords == null){
                                existingRelatedWords = new List<string>();
                            }
                            if(!existingRelatedWords.Contains(reverseWordValue)){
                                existingRelatedWords.Add(reverseWordValue);
                                relatedWordReverseLookup.TryUpdate(reverseWordKey, existingRelatedWords, existingRelatedWords);
                            }
                        }
                        else {
                            relatedWordReverseLookup.TryAdd(reverseWordKey, new List<string>{reverseWordValue});
                        }
                    }
                }

                Console.Write("shutter:" + relatedWordReverseLookup["shutter"]);
                
                relatedWordsToEmojiDictionaryKeysDictionary = relatedWordReverseLookup;
            }
        }

        public async Task AddRelatedWordsToEmojiDictionary() {
            await CreateRelatedWordsDictionary();
            Console.WriteLine("Writing related words to file.");
            PersistRelatedWordDictionaryToJsonFile(EMOJI_RELATED_WORDS_JSON_FILE_PATH);
        }

        private void PersistRelatedWordDictionaryToJsonFile(string filePath) {
            string json = JsonConvert.SerializeObject(relatedWordsDictionary, Formatting.Indented);
            Console.WriteLine("Saving json (of length " + json.Length + ") to: " + filePath);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Saved");
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
                    AddToRelatedWordsDictionary(key, relatedWords);
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
            if (relatedWordsDictionary.ContainsKey(key)) {
                var currentWords = relatedWordsDictionary[key].ToList();

                currentWords.AddRange(relatedWords.Except(currentWords));
                relatedWordsDictionary.TryUpdate(key, currentWords, currentWords);
            }
            else {
                relatedWordsDictionary.TryAdd(key, relatedWords);
            }
        }
    }
}