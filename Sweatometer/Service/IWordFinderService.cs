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
        /// Return a collection of options that relate to the merge of these two words,
        /// where the fixed word is never changed but the pivot word will be amended in order
        /// to work in the fixed word.
        /// </summary>
        /// <param name="fixedWord">Word to stay the same and work into the second word.</param>
        /// <param name="pivotWord">Pivot word to search against.</param>
        /// <returns>Collection of merge word options.</returns>
        Task<ICollection<SimilarWord>> MergeWords(string fixedWord, string pivotWord);
    }
}
