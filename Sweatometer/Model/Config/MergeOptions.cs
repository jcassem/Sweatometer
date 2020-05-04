namespace Sweatometer
{
    public class MergeOptions
    {
        /// <summary>
        /// Whether the merge method should consider synonyms of the inject word.
        /// </summary>
        public bool CheckSynonymsOfInjectWord { get; set; } = true;

        /// <summary>
        /// Whether the merge method should consider synonyms of the parent word.
        /// </summary>
        public bool CheckSynonymsOfParentWord { get; set; } = true;

        /// <summary>
        /// Whether the merge method should return the first result it finds.
        /// </summary>
        public bool ReturnOnFirstResult { get; set; } = false;

        /// <summary>
        /// Minimum score for synonyms to be considered. Others will be ignored/discarded.
        /// </summary>
        public int MinimumMeanScoreForSynoymns { get; set; } = 70000;

        /// <summary>
        /// Maximum words to consider that have a similar meaning to the inject word.
        /// </summary>
        public int MaxWordsToMeanLike { get; set; } = 20;

        /// <summary>
        /// Maximum words to consider that sound similar to the inject word.
        /// </summary>
        public int MaxWordsToSoundLike { get; set; } = 10;

        /// <summary>
        /// Maximum words to consider that have a similar spelling to the inject word.
        /// </summary>
        public int MaxWordsToSpellLike { get; set; } = 10;

        /// <summary>
        /// Merge results are hyphenated e.g. Egg-cellent.
        /// </summary>
        public bool MergeResultsAreHyphenated { get; set; } = true;
    }
}
