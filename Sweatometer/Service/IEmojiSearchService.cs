using System.Collections.Generic;
using System.Threading.Tasks;
using Sweatometer.Model;

namespace Sweatometer.Service
{
    /// <summary>
    /// Provides services to search for emojis from words.
    /// </summary>
    public interface IEmojiSearchService
    {
        /// <summary>
        /// Returns a collection of emojis that match search word.
        /// </summary>
        /// <param name="searchWord">Word to search emojis against.</param>
        /// <returns>Collection of emojis that match provided word.</returns>
        Task<ICollection<Emoji>> FindEmojisThatMatch(string searchWord);
    }
}
