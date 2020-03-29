using System.Collections.Generic;
using System.IO;

namespace Sweatometer
{
    public static class EmojiLoader
    {
        private static Dictionary<string, List<string>> emojiDictionary { get; set; }
        public static Dictionary<string, List<string>> EmojiDictionary {
            get {
                if(emojiDictionary == null)
                {
                    emojiDictionary = GetEmojisFromFile();
                }

                return emojiDictionary;
            }
        }

        public static void LoadEmojis()
        {
            emojiDictionary = GetEmojisFromFile();
        }

        private static Dictionary<string, List<string>> GetEmojisFromFile()
        {
            var emojiDictionary = new Dictionary<string, List<string>>();

            var filePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString()+ "Data/emoji.txt";
            StreamReader reader = File.OpenText(filePath);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');
                string emoji = null;

                foreach (string item in items)
                {
                    if(emoji == null)
                    {
                        emoji = item;
                    }
                    else
                    {
                        if (emojiDictionary.ContainsKey(item))
                        {
                            emojiDictionary[item].Add(emoji);
                        }
                        else
                        {
                            emojiDictionary.Add(item, new List<string> { emoji });
                        }
                    }
                }
            }

            return emojiDictionary;
        }
    }
}
