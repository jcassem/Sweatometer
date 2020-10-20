using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sweatometer.Model;
using Sweatometer.Service.Word;

namespace Sweatometer.Service.Merge {
    ///<inheritdoc/>
    public class MergeService : IMergeService {
        private readonly ILogger<MergeService> logger;

        private readonly IWordFinderService wordFinderService;

        private readonly IOptions<MergeOptions> mergeOptions;

        public IWordFinderService WordFinderService => wordFinderService;

        public MergeService(
            ILogger<MergeService> logger,
            IOptions<MergeOptions> mergeOptions,
            IWordFinderService wordFinderService) {
            this.logger = logger;
            this.wordFinderService = wordFinderService;
            this.mergeOptions = mergeOptions;
        }

        ///<inheritdoc/>
        public async Task<ICollection<MergedWord>> MergeWords(string parentWord, string injectWord) {
            return await MergeWords(
                parentWord,
                injectWord,
                mergeOptions?.Value?.ReturnOnFirstResult == true ? ResultSet.FIRST_RESULT_ONLY : ResultSet.ALL_RESULTS,
                mergeOptions?.Value?.CheckSynonymsOfInjectWord == true ? SynonymsOfInjectWord.INCLUDE : SynonymsOfInjectWord.EXCLUDE,
                mergeOptions?.Value?.CheckSynonymsOfParentWord == true ? SynonymsOfParentWord.INCLUDE : SynonymsOfParentWord.EXCLUDE
            );
        }

        ///<inheritdoc/>
        public async Task<ICollection<MergedWord>> MergeWords(
            string parentWord,
            string injectWord,
            ResultSet returnOnFirstResult,
            SynonymsOfInjectWord checkSynonymsOfInjectWord,
            SynonymsOfParentWord checkSynonymsOfParentWord
        ) {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var mappedPairs = await FindMergeWords(
                parentWord,
                injectWord,
                returnOnFirstResult,
                checkSynonymsOfInjectWord,
                checkSynonymsOfParentWord
            );

            watch.Stop();
            logger.LogInformation("FindMergeWords Runtime: " + watch.ElapsedMilliseconds + " milliseconds");

            return mappedPairs;
        }

        private async Task<ICollection<MergedWord>> FindMergeWords(
            string parentWord,
            string injectWord,
            ResultSet returnOnFirstResult,
            SynonymsOfInjectWord checkSynonymsOfInjectWord,
            SynonymsOfParentWord checkSynonymsOfParentWord

        ) {
            parentWord = parentWord.ToLower();
            injectWord = injectWord.ToLower();

            var mappedPairs = new List<MergedWord>();

            // Create a list of synonyms for the injected word
            var injectWords = new List<string>();
            injectWords.Add(injectWord);

            if (checkSynonymsOfInjectWord.Equals(SynonymsOfInjectWord.INCLUDE)) {
                injectWords.AddRange(await GetFilteredSynonymsOfWord(injectWord));
            }

            // Create a list of synonyms for the parent word
            var parentWords = new List<string>();
            parentWords.Add(parentWord);

            if (checkSynonymsOfParentWord.Equals(SynonymsOfParentWord.INCLUDE)) {
                parentWords.AddRange(await GetFilteredSynonymsOfWord(parentWord));
            }

            // Go through each inject word option (synonyms) and each of their related words (sounds like, spells like)
            foreach (var selectedinjectWord in injectWords) {
                var pivotOptions = await GetFilteredSimilarWordsFromWord(selectedinjectWord);

                // Process each similar word option and the word replacements for these words.
                foreach (var similarWordOption in pivotOptions) {
                    foreach (string parentWordOption in parentWords) {
                        foreach (string wordAttempt in FindCommonCharacterReplacements(similarWordOption.Word)
                            .Where(w => parentWordOption.Contains(w))) {
                            var replaceStartIndex = parentWordOption.IndexOf(wordAttempt, StringComparison.Ordinal);
                            var replaceEndIndex = replaceStartIndex + wordAttempt.Length;

                            var word = parentWordOption.Substring(0, replaceStartIndex) +
                                selectedinjectWord +
                                parentWordOption.Substring(replaceEndIndex);

                            MergedWord match = new MergedWord {
                                Word = word,
                                Score = similarWordOption.Score,
                                InjectedWord = selectedinjectWord,
                                ParentWord = parentWordOption
                            };

                            if (!mappedPairs.Any(x => x.Word == match.Word)) {
                                mappedPairs.Add(match);

                                if (returnOnFirstResult.Equals(ResultSet.FIRST_RESULT_ONLY)) {
                                    return mappedPairs;
                                }
                            }
                        }
                    }
                }
            }

            return mappedPairs;
        }

        /// <summary>
        /// Returns a filtered list of synonyms from the provided word.
        /// </summary>
        /// <param name="sourceWord">Word to search against.</param>
        /// <returns>Filtered list of synonyms.</returns>
        private async Task<IEnumerable<string>> GetFilteredSynonymsOfWord(string sourceWord) {
            var synonyms = await WordFinderService.GetWordsToMeanLikeAsync(sourceWord);

            return synonyms
                .Where(s => s.Score > mergeOptions.Value.MinScoreForSynoymns)
                .OrderByDescending(s => s.Score)
                .Take(mergeOptions.Value.MaxWordsToMeanLike)
                .Select(s => s.Word)
                .Distinct();
        }

        /// <summary>
        /// Provides a list of alternative word options that sound or spell like the provided one.
        /// </summary>
        /// <param name="sourceWord">Word to search against.</param>
        /// <returns>Filtered down list of options based of <code>mergeWordOptions</code></returns>
        private async Task<IList<SimilarWord>> GetFilteredSimilarWordsFromWord(string sourceWord) {
            var injectWordSoundsLikeOptions = await WordFinderService.GetWordsThatSoundLikeAsync(sourceWord);
            var filtedinjectWordSoundsLikeOptions = injectWordSoundsLikeOptions
                .OrderByDescending(s => s.Score)
                .Take(mergeOptions?.Value?.MaxWordsToSoundLike ?? injectWordSoundsLikeOptions.Count);

            var injectWordSpellsLikeOptions = await WordFinderService.GetWordsToSpellLikeAsync(sourceWord);
            var filteredinjectWordSpellsLikeOptions = injectWordSpellsLikeOptions
                .OrderByDescending(s => s.Score)
                .Take(mergeOptions?.Value?.MaxWordsToSpellLike ?? injectWordSoundsLikeOptions.Count);

            return filtedinjectWordSoundsLikeOptions.Concat(filteredinjectWordSpellsLikeOptions).ToList();
        }

        /// <summary>
        /// Yields an altered version of the provided word with some characters replaced
        /// to allow for easier word comparision e.g. removing double letters and inter-changing 'ph' and 'f'.
        /// </summary>
        /// <param name="sourceWord">Word to process.</param>
        /// <returns>Processed words with character replacements.</returns>
        public static IEnumerable<string> FindCommonCharacterReplacements(string sourceWord) {
            yield return RemoveDoubleLettersFromString(sourceWord);

            yield return sourceWord.Replace("ph", "f");
            yield return sourceWord.Replace("f", "ph");

            yield return sourceWord[0.. ^ 1];
        }

        /// <summary>
        /// Removes double letters from a string to allow easier word comparison.
        /// E.g. Glass => Glas
        /// </summary>
        /// <param name="doubleLetterWord">Word to remove double letters (if any) from.</param>
        /// <returns>Processed word.</returns>
        public static string RemoveDoubleLettersFromString(string doubleLetterWord) {
            var characters = doubleLetterWord.ToCharArray();
            char lastChar = characters[0];
            string singleLetterWord = lastChar.ToString();

            foreach (char letter in characters) {
                if (letter != lastChar) {
                    singleLetterWord += letter;
                    lastChar = letter;
                }
            }

            return singleLetterWord;
        }
    }
}