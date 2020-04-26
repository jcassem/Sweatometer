using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sweatometer.Service;

namespace Sweatometer
{
    public class EmojiLoader : IEmojiLoader
    {
        private static readonly string EMOJI_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/emoji.txt";

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

        public async Task LoadEmojisAsync()
        {
            await LoadEmojisFromFile();
        }

        private static async Task LoadEmojisFromFile()
        {
            emojiDictionary = new Dictionary<string, List<string>>();

            using (StreamReader reader = File.OpenText(EMOJI_FILE_PATH))
            {
                string line = await reader.ReadLineAsync();
                while (line != null)
                {
                    string[] items = line.Split(',');
                    string emoji = null;

                    foreach (string item in items)
                    {
                        var trimmedItem = item.Trim();

                        if(emoji == null)
                        {
                            emoji = trimmedItem;
                        }
                        else
                        {
                            AddToEmojiDictionary(trimmedItem, new List<string>{emoji});
                        }
                    }

                    line = await reader.ReadLineAsync();
                }
            }
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
