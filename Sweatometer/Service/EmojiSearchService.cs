using System;
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
        public async Task<ICollection<Emoji>> FindEmojisThatMatch(string searchWord)
        {
            var emojis = new List<Emoji>();
            var keys = EmojiLoader.EmojiDictionary.Keys;

            await Task.Run(() => {
                var matchedKeys = keys.Where(key => ContainsAllWords(key, searchWord));

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
