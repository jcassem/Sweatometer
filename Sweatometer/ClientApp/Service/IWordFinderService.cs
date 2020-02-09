using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
