using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sweatometer.Service;
using System.Threading;
using Microsoft.Extensions.Options;

namespace Sweatometer
{
    public class EmojiLoader : IEmojiLoader
    {
        private static readonly string EMOJI_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/emoji.json";
        private static readonly string EMOJI_FULL_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/emojiFull.json";

        private static ConcurrentDictionary<string, List<string>> emojiDictionary { get; set; }
        public static ConcurrentDictionary<string, List<string>> EmojiDictionary {
            get {
                return emojiDictionary;
            }
        }

        private readonly IWordFinderService wordFinderService;

        private readonly IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions;

        public EmojiLoader(
            IWordFinderService wordFinderService,
            IOptions<EmojiRelatedWordOptions> emojiRelatedWordOptions
        ){
            this.wordFinderService = wordFinderService;
            this.emojiRelatedWordOptions = emojiRelatedWordOptions;
        }

        public  void LoadEmojis()
        {
            LoadEmojisFromFile();
        }

        private static void LoadEmojisFromFile()
        {
            var jsonRaw = File.ReadAllText(EMOJI_FULL_JSON_FILE_PATH);
            var values = JsonConvert.DeserializeObject<ConcurrentDictionary<string, List<string>>>(jsonRaw);
            if(values != null){
                emojiDictionary = values;
            }
        }

        private void persistEmojiDictionaryToJsonFile(string filePath){
            string json = JsonConvert.SerializeObject(emojiDictionary, Formatting.Indented);

            File.WriteAllText(filePath, json);
        }

        public async Task AddRelatedWordsToEmojiDictionaryAsync(){
            await AddRelatedWords();
            Console.WriteLine("Writing related emojis to file.");
            persistEmojiDictionaryToJsonFile(EMOJI_FULL_JSON_FILE_PATH);
        }

        private async Task AddRelatedWords(){
            var initialKeys = new List<string>(emojiDictionary.Keys);
            int sum = initialKeys.Count;
            int count = 0;
            bool includeSynonyms = emojiRelatedWordOptions?.Value?.MaxAmountSynonyms > 0 == true;
            bool includeRelatedWords = emojiRelatedWordOptions?.Value?.MaxAmountRelatedWords > 0 == true;
            int minScoreForSynonyms = emojiRelatedWordOptions?.Value?.MinScoreForSynoymns ?? 0;
            int minScoreForRetaltedWords = emojiRelatedWordOptions?.Value?.MinScoreForRelatedWords ?? 0;

            foreach(var key in initialKeys)
            {
                var emojis = emojiDictionary[key];
                
                if(includeSynonyms){
                    var foundWords =  await wordFinderService.GetWordsToMeanLikeAsync(key);

                    if(foundWords != null){
                        var topResults = foundWords
                            .Where(x => x.Score >= minScoreForSynonyms)
                            .OrderByDescending(s => s.Score)
                            .Take(emojiRelatedWordOptions?.Value?.MaxAmountSynonyms ?? 10);

                        foreach(var relatedWord in topResults){
                            AddToEmojiDictionary(relatedWord.Word, emojis);
                        }
                    }
                }

                if(includeRelatedWords){
                    var foundWords =  await wordFinderService.GetRelatedTriggerWords(key);

                    if(foundWords != null){
                        var topResults = foundWords
                            .Where(x => x.Score >= minScoreForRetaltedWords)
                            .OrderByDescending(s => s.Score)
                            .Take(emojiRelatedWordOptions?.Value?.MaxAmountRelatedWords ?? 10);

                        foreach(var relatedWord in topResults){
                            AddToEmojiDictionary(relatedWord.Word, emojis);
                        }
                    }
                }
                
                count++;
                Console.WriteLine($"{count}/{sum} related words processed.");
            }
        }

        private static void AddToEmojiDictionary(string key, List<string> emojis){
            if (emojiDictionary.ContainsKey(key))
            {
                var currentEmojis = emojiDictionary[key].ToList();
                currentEmojis.AddRange(emojis.Except(currentEmojis));
                emojiDictionary.TryUpdate(key, currentEmojis, currentEmojis);
            }
            else
            {
                emojiDictionary.TryAdd(key, emojis);
            }
        }
    }
}
