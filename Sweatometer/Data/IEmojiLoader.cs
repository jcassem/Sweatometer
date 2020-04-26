using System.Threading.Tasks;

namespace Sweatometer
{
    public interface IEmojiLoader
    {
        Task LoadEmojisAsync();

        Task AddRelatedWordsToEmojiDictionaryAsync();
    }
}
