using System.Collections.Generic;
using System.Threading.Tasks;
using Sweatometer.Model;

namespace Sweatometer.Service.EmojiSearch {
    /// <summary>
    /// Provides services to search for emojis from words.
    /// </summary>
    public interface IEmojiSearchService {
        /// <summary>
        /// Returns a collection of emojis that match search word.
        /// </summary>
        /// <param name="searchTerm">Word(s) to search emojis against.</param>
        /// <returns>Collection of emojis that match provided word.</returns>
        Task<ICollection<Emoji>> FindEmojisThatMatch(string searchTerm);

        /// <summary>
        /// Returns a map of words and the collection of emojis that match it.
        /// </summary>
        /// <param name="searchTerm">Word(s) to search emojis against.</param>
        /// <returns>Collection of emojis that match provided word.</returns>
        Task<IDictionary<string, ICollection<Emoji>>> FindSetOfEmojisThatMatch(string searchTerm);
    }
}