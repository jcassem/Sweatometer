﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sweatometer.Model;

namespace Sweatometer.Service
{
    /// <summary>
    /// Provides services to retreive related words using the DataMuse api.
    /// </summary>
    public interface IWordFinderService
    {
        /// <summary>
        /// Returns a collection of words that sound similar to the one provided.
        /// </summary>
        /// <param name="wordToSoundLike">Word to find similar words to.</param>
        /// <returns>Collection of words similar to the one provided.</returns>
        Task<ICollection<SimilarWord>> GetWordsThatSoundLikeAsync(string wordToSoundLike);

        /// <summary>
        /// Returns a collection of words that have a similar meaning to the one provided.
        /// </summary>
        /// <param name="wordToMeanLike">Word to search against.</param>
        /// <returns>Collection of words with similar meanings to the one provided.</returns>
        Task<ICollection<SimilarWord>> GetWordsToMeanLikeAsync(string wordToMeanLike);

        /// <summary>
        /// Returns a collection of words that spell similar to the one provided.
        /// </summary>
        /// <param name="wordToSpellLike">Word to find similar words to.</param>
        /// <returns>Collection of words that spell similar to the one provided.</returns>
        Task<ICollection<SimilarWord>> GetWordsToSpellLikeAsync(string wordToSpellLike);

        /// <summary>
        /// Returns a collection of suggested words to complete the one provided.
        /// </summary>
        /// <param name="toSuggestFrom">Word/Partial word to search against.</param>
        /// <returns>Collection of words suggestions based off the one provided.</returns>
        Task<ICollection<SimilarWord>> GetSuggestedWordsAsync(string toSuggestFrom);

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
