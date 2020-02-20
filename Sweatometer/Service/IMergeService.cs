using System.Collections.Generic;
using System.Threading.Tasks;
using Sweatometer.Model;

namespace Sweatometer.Service
{
    /// <summary>
    /// Provides services to merge words.
    /// </summary>
    public interface IMergeService
    {
        /// <summary>
        /// Generates a collection of variations of the <code>parentWord</code>
        /// with the <code>injectWord</code> (or a similar verison of it) inserted within it.
        /// </summary>
        /// <param name="parentWord">Word to inject into.</param>
        /// <param name="injectWord">Word to inject.</param>
        /// <returns>Collection of merged words.</returns>
        Task<ICollection<MergedWord>> MergeWords(string parentWord, string injectWord);
    }
}
