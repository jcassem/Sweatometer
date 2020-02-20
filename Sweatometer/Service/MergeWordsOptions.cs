namespace Sweatometer.Service
{
    public class MergeWordsOptions
    {

        public bool CheckSynonyms { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of synonyms to try.
        /// </summary>
        public int MaxSynonyms { get; set; } = 5;

        /// <summary>
        /// Gets or sets the minimum score a synonym should have to be considered for merge.
        /// </summary>
        public int MeansLikeMinimumScore { get; set; } = 70000;

        public int MaxWordsToSoundLike { get; set; } = 10;

        public int MaxWordsToSpellLike { get; set; } = 10;

        public static MergeWordsOptions Default
        {
            get
            {
                return new MergeWordsOptions();
            }
        }
    }
}
