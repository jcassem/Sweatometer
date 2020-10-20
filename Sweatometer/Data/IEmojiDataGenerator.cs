
using System.Threading.Tasks;

namespace Sweatometer.Data.Emoji
{
    public interface IEmojiDataGenerator
    {
        Task CreateRelatedWordsDictionary();
    }
}
