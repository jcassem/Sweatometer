using System.Threading.Tasks;

namespace Sweatometer
{
    public interface IEmojiLoader
    {
        void LoadEmojis();

        Task AddRelatedWordsToEmojiDictionaryAsync();
    }
}
