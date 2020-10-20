using System.Collections.Generic;
using Sweatometer.Model;

namespace Sweatometer.Data.Emoji {

    /// <summary>
    /// Provides static values containing emoji data.
    /// </summary>
    public class EmojiData {

        /// <summary>
        /// Constructor - private as this class is intended to be static (but has non-private attributes).
        /// </summary>
        private EmojiData(){}

        /// <summary>
        /// Map of emoji names and their icon options.
        /// </summary>
        /// <value>Emoji name/icon map.</value>
        protected internal static Dictionary<string, List<string>> emojiDictionary { get; set; }

        /// <summary>
        /// Map of emoji names and their icon options.
        /// </summary>
        /// <value>Emoji name/icon map.</value>
        public static Dictionary<string, List<string>> EmojiDictionary {
            get {
                return emojiDictionary;
            }
        }


        /// <summary>
        /// Map of related words to the emoji dictionary names generated using the WordSearchService.
        /// </summary>
        /// <value>Related word map of emoji icon names and their related words.</value>
        protected internal static Dictionary<string, List<SimilarWord>> relatedWordsDictionary { get; set; }
        
        /// <summary>
        /// Map of related words to the emoji dictionary names generated using the WordSearchService.
        /// </summary>
        /// <value>Related word map of emoji icon names and their related words.</value>
        public static Dictionary<string, List<SimilarWord>> RelatedWordsDictionary {
            get {
                return relatedWordsDictionary;
            }
        }


        /// <summary>
        /// Lookup map to find what emoji dictionary keys relate to the related word keys in this dictionary.
        /// </summary>
        /// <value>Map of related words and the emoji dictionary keys they relate to.</value>
        protected internal static Dictionary<string, List<SimilarWord>> relatedWordsToEmojiDictionaryKeysDictionary { get; set; }

        /// <summary>
        /// Lookup map to find what emoji dictionary keys relate to the related word keys in this dictionary.
        /// </summary>
        /// <value>Map of related words and the emoji dictionary keys they relate to.</value>
        public static Dictionary<string, List<SimilarWord>> RelatedWordsToEmojiDictionaryKeysDictionary {
            get {
                return relatedWordsToEmojiDictionaryKeysDictionary;
            }
        }
    }
}