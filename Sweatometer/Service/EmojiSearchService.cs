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
                var matchedKeys = keys.Where(key => WholeWordSearch(key, searchWord));

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

        public static bool WholeWordSearch(string input, string searchTerm){
            var pattern = @"\b" + Regex.Escape(searchTerm.ToLower()) + @"\b";
            return Regex.IsMatch(input.ToLower(), pattern); 
        }
    }
}
