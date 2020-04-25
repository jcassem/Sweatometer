using System.Collections.Generic;
using System.IO;
using Sweatometer.Service;

namespace Sweatometer
{
    public interface IEmojiLoader
    {
        void LoadEmojis();

        void AddRelatedWordsToEmojiDictionary();
    }
}
