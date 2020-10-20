using System.Collections.Generic;
using Sweatometer.Model;

namespace Sweatometer.Data.Emoji {

    public class EmojiData {

        private EmojiData(){}

        protected internal static Dictionary<string, List<string>> emojiDictionary { get; set; }
        public static Dictionary<string, List<string>> EmojiDictionary {
            get {
                return emojiDictionary;
            }
        }

        protected internal static Dictionary<string, List<SimilarWord>> relatedWordsDictionary { get; set; }
        public static Dictionary<string, List<SimilarWord>> RelatedWordsDictionary {
            get {
                return relatedWordsDictionary;
            }
        }

        protected internal static Dictionary<string, List<string>> relatedWordsToEmojiDictionaryKeysDictionary { get; set; }
        public static Dictionary<string, List<string>> RelatedWordsToEmojiDictionaryKeysDictionary {
            get {
                return relatedWordsToEmojiDictionaryKeysDictionary;
            }
        }
    }
}