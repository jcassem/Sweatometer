using System.Collections.Generic;
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
        /// Returns a collection of "Triggers" words that are statistically associated with 
        /// the query word in the same piece of text.
        /// </summary>
        /// <param name="toRelateFrom">Word to search against.</param>
        /// <returns>Collection of words that are related to the one provided.</returns>
        Task<ICollection<SimilarWord>> GetRelatedTriggerWords(string toRelateFrom);
    }
}
