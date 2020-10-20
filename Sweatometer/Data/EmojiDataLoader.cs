using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sweatometer.Model;

namespace Sweatometer.Data.Emoji {

    public class EmojiDataLoader {
        protected internal static readonly string EMOJI_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/Resources/emoji.json";
         protected internal static readonly string EMOJI_RELATED_WORDS_JSON_FILE_PATH = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Data/Resources/emojiRelatedWords.json";

        private EmojiDataLoader() { }

        public static void LoadEmojiDataFromFile() {
            LoadEmojiDictionaryFromFile();
            LoadEmojiRelatedWordsFromFile();
            GenerateRelatedWordsToEmojiDictionaryKeysDictionary();
        }

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
                if(EmojiData.emojiDictionary == null){
                    EmojiData.emojiDictionary = new Dictionary<string, List<string>>();
                }
            }
        }

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
                if(EmojiData.relatedWordsDictionary == null){
                    EmojiData.relatedWordsDictionary = new Dictionary<string, List<SimilarWord>>();
                }
            }
        }

        private static void GenerateRelatedWordsToEmojiDictionaryKeysDictionary() {
            if(EmojiData.RelatedWordsToEmojiDictionaryKeysDictionary == null){
                var relatedWordReverseLookup = new Dictionary<string, List<string>>();

                foreach(var relatedWordKey in EmojiData.RelatedWordsDictionary.Keys){
                    var relatedWords = EmojiData.RelatedWordsDictionary[relatedWordKey];
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
                                relatedWordReverseLookup[reverseWordKey] = existingRelatedWords;
                            }
                        }
                        else {
                            relatedWordReverseLookup.TryAdd(reverseWordKey, new List<string>{reverseWordValue});
                        }
                    }
                }
                
                EmojiData.relatedWordsToEmojiDictionaryKeysDictionary = relatedWordReverseLookup;
            }
        }
    }
}