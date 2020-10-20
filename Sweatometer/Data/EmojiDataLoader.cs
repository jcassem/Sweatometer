using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sweatometer.Model;

namespace Sweatometer.Data.Emoji {

    /// <summary>
    /// Provides static methods to load emoji data.
    /// </summary>
    public class EmojiDataLoader {

        /// <summary>
        /// File path to the persisted emoji dictionary.
        /// </summary>
        protected internal static readonly string EMOJI_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/Resources/emoji.json";

        /// <summary>
        /// File path to the persisted related words dictionary.
        /// </summary>
        protected internal static readonly string EMOJI_RELATED_WORDS_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/Resources/emojiRelatedWords.json";

        /// <summary>
        /// Constructor - private as this class is intended to be static (but has non-private attributes).
        /// </summary>
        private EmojiDataLoader() { }

        /// <summary>
        /// Loads all required emoji data from file.
        /// </summary>
        public static void LoadEmojiDataFromFile() {
            LoadEmojiDictionaryFromFile();
            LoadEmojiRelatedWordsFromFile();
            GenerateRelatedWordsToEmojiDictionaryKeysDictionary();
        }

        /// <summary>
        /// Loads the emoji dictionary data from file and sets it in EmojiData.
        /// </summary>
        private static void LoadEmojiDictionaryFromFile() {
            try{
                var jsonRaw = File.ReadAllText(EMOJI_JSON_FILE_PATH);
                var values = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonRaw);
                if (values != null) {
                    EmojiData.emojiDictionary = values;
                }
            }
            catch(FileNotFoundException){
                // ignore for now
            }
            finally{
                if(EmojiData.EmojiDictionary == null){
                    EmojiData.emojiDictionary = new Dictionary<string, List<string>>();
                }
            }
        }

        /// <summary>
        /// Loads the related word dictionary data from file and sets it in EmojiData.
        /// </summary>
        private static void LoadEmojiRelatedWordsFromFile() {
            try{
                var jsonRaw = File.ReadAllText(EMOJI_RELATED_WORDS_JSON_FILE_PATH);
                var values = JsonConvert.DeserializeObject<Dictionary<string, List<SimilarWord>>>(jsonRaw);
                if (values != null) {
                    EmojiData.relatedWordsDictionary = values;
                }
            }
            catch(FileNotFoundException){
                // ignore for now
            }
            finally{
                if(EmojiData.RelatedWordsDictionary == null){
                    EmojiData.relatedWordsDictionary = new Dictionary<string, List<SimilarWord>>();
                }
            }
        }

        /// <summary>
        /// Generates the related word lookup dictionary and saves it to EmojiData.
        /// </summary>
        private static void GenerateRelatedWordsToEmojiDictionaryKeysDictionary() {
            if(EmojiData.RelatedWordsToEmojiDictionaryKeysDictionary == null){
                var relatedWordReverseLookup = new Dictionary<string, List<SimilarWord>>();

                foreach(var relatedWordKey in EmojiData.RelatedWordsDictionary.Keys){
                    var relatedWords = EmojiData.RelatedWordsDictionary[relatedWordKey];
                    foreach(var relatedWord in relatedWords){
                        var reverseWordKey = relatedWord.Word;
                        var reverseWordValue = relatedWordKey;

                        if(relatedWordReverseLookup.ContainsKey(reverseWordKey)){
                            var existingRelatedWords = relatedWordReverseLookup[reverseWordKey];

                            if(existingRelatedWords == null){
                                existingRelatedWords = new List<SimilarWord>();
                            }
                            
                            if(!existingRelatedWords.Any(x => x.Word.Equals(reverseWordValue))){
                                existingRelatedWords.Add(new SimilarWord(reverseWordValue, relatedWord.Score));

                                relatedWordReverseLookup[reverseWordKey] = existingRelatedWords
                                        .OrderByDescending(x => x.Score)
                                        .ToList();
                            }
                        }
                        else {
                            var wordToAdd = new SimilarWord(reverseWordValue, relatedWord.Score);
                            relatedWordReverseLookup.TryAdd(reverseWordKey, new List<SimilarWord>{wordToAdd});
                        }
                    }
                }
                
                EmojiData.relatedWordsToEmojiDictionaryKeysDictionary = relatedWordReverseLookup;
            }
        }
    }
}