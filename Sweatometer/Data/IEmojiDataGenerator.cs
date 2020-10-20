
using System.Threading.Tasks;

namespace Sweatometer.Data.Emoji
{
    /// <summary>
    /// Allows the generation of emoji data files.
    /// </summary>
    public interface IEmojiDataGenerator
    {
        /// <summary>
        /// Generates a file containing all related words to the existing emoji dictionary.
        /// Subject to config this will look for synonyms and related words to each emoji icon description word 
        /// and save this against the new related words file. Limitations on word score and amount per emoji are 
        /// also set by config.
        /// </summary>
        /// <returns>Task.</returns>
        Task GenerateRelatedWordsDictionaryFile();
    }
}
