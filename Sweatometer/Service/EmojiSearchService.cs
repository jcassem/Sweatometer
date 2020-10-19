using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sweatometer.Model;
using Sweatometer.Service;

namespace Sweatometer
{
    ///<inheritdoc/>
    public class EmojiSearchService : IEmojiSearchService
    {
        ///<inheritdoc/>
        public async Task<IDictionary<string,ICollection<Emoji>>> FindSetOfEmojisThatMatch(string searchTerm)
        {
            var foundEmojiDictionary = new Dictionary<string, ICollection<Emoji>>();

            var result = await FindEmojisThatMatch(searchTerm);

            if(!result.Any()){
                var searchWords = searchTerm.Split(' ');
                foreach(var searchWord in searchWords){
                    var wordResult = await FindEmojisThatMatch(searchWord);
                    foundEmojiDictionary.Add(searchWord, wordResult);
                }
            }
            else{
                foundEmojiDictionary.Add(searchTerm, result);
            }

            return foundEmojiDictionary;
        }

        ///<inheritdoc/>
        public async Task<ICollection<Emoji>> FindEmojisThatMatch(string searchTerm)
        {
            var emojis = new List<Emoji>();
            var keys = EmojiLoader.EmojiDictionary.Keys;

            await Task.Run(() => {
                var matchedKeys = new List<string>();
                matchedKeys.AddRange(FindMatchesForSearchTermIn(searchTerm, keys));


                // search related words if none found
                if(!matchedKeys.Any()){
                    foreach(var relatedWordKey in EmojiLoader.RelatedWordsForEmojiDictionary.Keys){
                        var relatedWords = EmojiLoader.RelatedWordsForEmojiDictionary[relatedWordKey];
                        var relatedWordMatches = FindMatchesForSearchTermIn(searchTerm, relatedWords.Select(sw => sw.Word).ToList());
                        if(relatedWordMatches.Any()){
                            matchedKeys.Add(relatedWordKey);
                        }
                    }

                    // todo replace with reverse key lookup for performance increase
                    // var matchedRelatedKeys = FindMatchesForSearchTermIn(searchTerm, EmojiLoader.KeyLookupDictionaryForRelatedWords.Keys);
                    // var matchedRelatedWords = EmojiLoader.RelatedWordsForEmojiDictionary.Where(x => matchedRelatedKeys.Contains(x.Key));
                    // foreach(var matchedWord in matchedRelatedWords){
                    //     matchedKeys.Add(matchedWord.Key);
                    // }
                }
                
                if (matchedKeys.Any())
                {
                    foreach (String key in matchedKeys)
                    {
                        foreach(string emojiIcon in EmojiLoader.EmojiDictionary[key].ToList()){
                            var emojiToAdd = new Emoji(emojiIcon, key);
                            var existingEmoji = emojis.FirstOrDefault(x => x.Icon.Equals(emojiToAdd.Icon));
                            
                            if(existingEmoji != null){
                                if(!existingEmoji.Description.Split(", ").Contains(emojiToAdd.Description)){
                                    existingEmoji.Description += ", " + emojiToAdd.Description;
                                }
                            }
                            else{
                                emojis.Add(emojiToAdd);
                            }
                        }
                        
                    }
                }
            });            

            return emojis;
        }

        /// <summary>
        /// Searches for whole or partial word matches for <paramref name="searchTerm"/> in <paramref name="toSearchIn"/>.
        /// </summary>
        /// <param name="searchTerm">Search term to look for.</param>
        /// <param name="toSearchIn">Collection of words to look in.</param>
        /// <returns>Matched entries from collection.</returns>
        public static List<string> FindMatchesForSearchTermIn(string searchTerm, ICollection<string> toSearchIn){
            var matchedKeys = toSearchIn.Where(key => key.ToLower().Equals(searchTerm.ToLower())).ToList();
            matchedKeys.AddRange(toSearchIn.Where(key => WholeWordSearch(key, searchTerm) && !matchedKeys.Contains(key)));
            matchedKeys.AddRange(toSearchIn.Where(key => ContainsAllWords(key, searchTerm) && !matchedKeys.Contains(key)));
            return matchedKeys;
        }

        /// <summary>
        /// Searches <paramref name="toSearchIn"/> for all the words in <paramref name="toSearchFor"/> in any order (case in-sensitive).
        /// </summary>
        /// <param name="toSearchIn">String to search for words in.</param>
        /// <param name="toSearchFor">String to search for.<toSearchIn></param>
        /// <returns>Whether <paramref name="toSearchIn"/> contains all the words in <paramref name="toSearchFor"/>.</returns>
        public static bool ContainsAllWords(string toSearchIn, string toSearchFor){
            if(!WholeWordSearch(toSearchIn, toSearchFor)){
                foreach(String word in toSearchFor.Split(' ')){
                    if(!WholeWordSearch(toSearchIn, word)){
                        return false;
                    }
                }
            }
                
            return true;
        }

        /// <summary>
        /// Checks if <paramref name="toSearchIn"/> contains <paramref name="toSearchFor"/> (ordered match).
        /// </summary>
        /// <param name="toSearchIn">String to search in.</param>
        /// <param name="toSearchFor">String to search for.</param>
        /// <returns>Whether <paramref name="toSearchIn"/> contains <paramref name="toSearchFor"/>.</returns>
        public static bool WholeWordSearch(string toSearchIn, string toSearchFor){
            var pattern = @"\b" + Regex.Escape(toSearchFor.ToLower()) + @"\b";
            return Regex.IsMatch(toSearchIn.ToLower(), pattern);
        }
    }
}
