using System.Collections.Generic;
using System.Threading.Tasks;
using Sweatometer.Model;

namespace Sweatometer.Service
{
    /// <summary>
    /// Provides services to test the sweat levels of a provided word combination/pun.
    /// </summary>
    public interface IMergeService
    {
        /// <summary>
        /// Generates a collection of variations of the <code>parentWord</code>
        /// with the <code>injectWord</code> (or a similar verison of it) inserted within it.
        /// This collection may contain one or more element depending on config.
        /// </summary>
        /// <param name="parentWord">Word to inject into.</param>
        /// <param name="injectWord">Word to inject.</param>
        /// <returns>Collection of merged words.</returns>
        Task<ICollection<MergedWord>> MergeWords(string parentWord, string injectWord);

        /// <summary>
        /// Finds the best merge word from the <code>parentWord</code> with <code>injectWord</code>
        /// (or a similar verison of it) inserted within it.
        /// </summary>
        /// <param name="parentWord">Word to inject into.</param>
        /// <param name="injectWord">Word to inject.</param>
        /// <returns>Best merge word.</returns>
        Task<ICollection<MergedWord>> FindBestMergeWord(string parentWord, string injectWord);

        /// <summary>
        /// Generates a collection of variations of the <code>parentWord</code>
        /// with the <code>injectWord</code> (or a similar verison of it) inserted within it.
        /// </summary>
        /// <param name="parentWord">Word to inject into.</param>
        /// <param name="injectWord">Word to inject.</param>
        /// <returns>Collection of merged words.</returns>
        Task<ICollection<MergedWord>> FindMergeWords(string parentWord, string injectWord);
    }
}
