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
        /// Return a collection of options that relate to the merge of these two words
        /// </summary>
        /// <param name="firstWord">First word for the merge.</param>
        /// <param name="secondWord">Second word for the merge.</param>
        /// <returns>Collection of merge word options.</returns>
        Task<ICollection<string>> MergeWords(string firstWord, string secondWord);
    }
}
