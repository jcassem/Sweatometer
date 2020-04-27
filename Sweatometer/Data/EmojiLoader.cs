using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sweatometer.Service;

namespace Sweatometer
{
    public class EmojiLoader : IEmojiLoader
    {
        private static readonly string EMOJI_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/emoji.json";

        private static Dictionary<string, List<string>> emojiDictionary { get; set; }
        public static Dictionary<string, List<string>> EmojiDictionary {
            get {
                return emojiDictionary;
            }
        }

        private readonly IWordFinderService wordFinderService;

        public EmojiLoader(IWordFinderService wordFinderService){
            this.wordFinderService = wordFinderService;
        }

        public  void LoadEmojis()
        {
            LoadEmojisFromFile();
        }

        private static void LoadEmojisFromFile()
        {
            var jsonRaw = File.ReadAllText(EMOJI_JSON_FILE_PATH);
            var values = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonRaw);
            if(values != null){
                emojiDictionary = values;
            }
        }

        private void persistEmojiDictionaryToJsonFile(){
            string json = JsonConvert.SerializeObject(emojiDictionary, Formatting.Indented);

            File.WriteAllText(EMOJI_JSON_FILE_PATH, json);
        }

        public async Task AddRelatedWordsToEmojiDictionaryAsync(){
            var initialKeys = new List<string>(emojiDictionary.Keys);

            foreach(string key in initialKeys){
                
                var emojis = emojiDictionary[key];
                var relatedWords =  await wordFinderService.GetRelatedTriggerWords(key);

                foreach(var relatedWord in relatedWords){
                    AddToEmojiDictionary(relatedWord.Word, emojis);
                }
            }
        }

        private static void AddToEmojiDictionary(string key, List<string> emojis){
            if (emojiDictionary.ContainsKey(key))
            {
                emojiDictionary[key].AddRange(emojis);
            }
            else
            {
                emojiDictionary.Add(key, emojis);
            }
        }
    }
}
