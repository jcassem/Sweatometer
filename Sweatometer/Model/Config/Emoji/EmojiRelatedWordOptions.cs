namespace Sweatometer {
    public class EmojiRelatedWordOptions {
        /// <summary>
        /// Minimum score for synonyms to be considered.
        /// </summary>
        public int MinScoreForSynoymns { get; set; } = 70000;

        /// <summary>
        /// Maximum amount of synonym words allowed per word search.
        /// </summary>
        public int MaxAmountSynonyms { get; set; } = 10;

        /// <summary>
        /// Minimum score for related words to be considered..
        /// </summary>
        public int MinScoreForRelatedWords { get; set; } = 1400;

        /// <summary>
        /// Maximum amount of related words allowed per word search.
        /// </summary>
        public int MaxAmountRelatedWords { get; set; } = 10;
    }
}