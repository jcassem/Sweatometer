using System;
using System.Collections.Generic;
using System.Linq;
using Sweatometer.Model;
using Sweatometer.Service;

namespace Sweatometer
{
    ///<inheritdoc/>
    public class EmojiSearchService : IEmojiSearchService
    {
        ///<inheritdoc/>
        public ICollection<Emoji> FindEmojisThatMatch(string searchWord)
        {
            var emojis = new List<Emoji>();

            var searchWordLowerCase = searchWord.ToLower();
            var keys = EmojiLoader.EmojiDictionary.Keys;
            var matchedKeys = keys.Where(key => key.ToLower().Contains(searchWordLowerCase) || searchWordLowerCase.Contains(key.ToLower()));

            if (matchedKeys.Any())
            {   
                foreach(String key in matchedKeys){
                    emojis.Add(new Emoji(EmojiLoader.EmojiDictionary[key].First(), key));
                }
            }

            return emojis;
        }
    }
}
