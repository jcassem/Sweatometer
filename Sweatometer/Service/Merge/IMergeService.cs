using System.Collections.Generic;
using System.Threading.Tasks;
using Sweatometer.Model;

namespace Sweatometer.Service.Merge {
    /// <summary>
    /// Provides services to test the sweat levels of a provided word combination/pun.
    /// </summary>
    public interface IMergeService {
        /// <summary>
        /// Generates a collection of variations of the <code>parentWord</code>
        /// with the <code>injectWord</code> (or a similar verison of it) inserted within it.
        /// Uses application defaults for merge search configuration.
        /// </summary>
        /// <param name="parentWord">Word to inject into.</param>
        /// <param name="injectWord">Word to inject.</param>
        /// <returns>Collection of merged words.</returns>
        Task<ICollection<MergedWord>> MergeWords(string parentWord, string injectWord);

        /// <summary>
        /// Generates a collection of variations of the <code>parentWord</code>
        /// with the <code>injectWord</code> (or a similar verison of it) inserted within it 
        /// with custom merge search configuration.
        /// </summary>
        /// <param name="parentWord">Word to inject into.</param>
        /// <param name="injectWord">Word to inject.</param>
        /// <param name="returnOnFirstResult">Return on the first result or continue to find them all.</param>
        /// <param name="checkSynonymsOfInjectWord">Include synonyms of injected word in merge search.</param>
        /// <param name="checkSynonymsOfParentWord">Include synonyms of parent word in merge search.</param>
        /// <returns>Collection of merged words.</returns>
        Task<ICollection<MergedWord>> MergeWords(
            string parentWord,
            string injectWord,
            ResultSet returnOnFirstResult,
            SynonymsOfInjectWord checkSynonymsOfInjectWord,
            SynonymsOfParentWord checkSynonymsOfParentWord
        );
    }
}