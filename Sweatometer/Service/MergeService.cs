using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sweatometer.Model;

namespace Sweatometer.Service
{
    ///<inheritdoc/>
    public class MergeService : IMergeService
    {
        private readonly ILogger<WordFinderService> logger;

        private readonly IWordFinderService wordFinderService;

        private readonly IOptions<MergeOptions> mergeOptions;

        public MergeService(
            ILogger<WordFinderService> logger,
            IOptions<MergeOptions> mergeOptions,
            IWordFinderService wordFinderService)
        {
            this.logger = logger;
            this.wordFinderService = wordFinderService;
            this.mergeOptions = mergeOptions;
        }

        ///<inheritdoc/>
        public async Task<ICollection<MergedWord>> MergeWords(string parentWord, string injectWord)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var mappedPairs = await MergeWords(parentWord, injectWord, mergeOptions?.Value?.ReturnOnFirstResult == true);

            watch.Stop();
            logger.LogInformation("FindBestMergeWord Runtime: " + watch.ElapsedMilliseconds + " milliseconds");

            return mappedPairs;
        }

        ///<inheritdoc/>
        public async Task<ICollection<MergedWord>> FindBestMergeWord(string parentWord, string injectWord)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var mappedPairs = await MergeWords(parentWord, injectWord, true);

            watch.Stop();
            logger.LogInformation("FindBestMergeWord Runtime: " + watch.ElapsedMilliseconds + " milliseconds");

            return mappedPairs;
        }

        ///<inheritdoc/>
        public async Task<ICollection<MergedWord>> FindMergeWords(string parentWord, string injectWord)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var mappedPairs = await MergeWords(parentWord, injectWord, false);

            watch.Stop();
            logger.LogInformation("FindMergeWords Runtime: " + watch.ElapsedMilliseconds + " milliseconds");

            return mappedPairs;
        }

        private async Task<ICollection<MergedWord>> MergeWords(string parentWord, string injectWord, bool returnOnFirstResult)
        {
            parentWord = parentWord.ToLower();
            injectWord = injectWord.ToLower();

            var mappedPairs = new List<MergedWord>();

            // Create a list of synonyms for the injected word
            var injectWords = new List<string>();
            injectWords.Add(injectWord);

            if (mergeOptions?.Value?.CheckSynonyms == true)
            {
                injectWords.AddRange(await GetFilteredSynonymsOfWord(injectWord));
            }

            // Go through each inject word option (synonyms) and each of their related words (sounds like, spells like)
            foreach (var selectedinjectWord in injectWords)
            {
                var pivotOptions = await GetFilteredSimilarWordsFromWord(selectedinjectWord);

                // Process each similar word option and the word replacements for these words.
                foreach (var similarWordOption in pivotOptions)
                {
                    foreach (string wordAttempt in FindCommonCharacterReplacements(similarWordOption.Word)
                        .Where(w => parentWord.Contains(w)))
                    {
                        var replaceStartIndex = parentWord.IndexOf(wordAttempt, StringComparison.Ordinal);
                        var replaceEndIndex = replaceStartIndex + wordAttempt.Length;

                        MergedWord match = new MergedWord
                        {
                            Word = parentWord.Substring(0, replaceStartIndex) + selectedinjectWord + parentWord.Substring(replaceEndIndex),
                            Score = similarWordOption.Score,
                            InjectedWord = wordAttempt,
                            ParentWord = parentWord
                        };

                        if (!mappedPairs.Any(x => x.Word == match.Word))
                        {
                            mappedPairs.Add(match);

                            if (returnOnFirstResult)
                            {
                                return mappedPairs;
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
        private async Task<IEnumerable<string>> GetFilteredSynonymsOfWord(string sourceWord)
        {
            var synonyms = await wordFinderService.GetWordsToMeanLikeAsync(sourceWord);

            return synonyms
                .Where(s => s.Score > mergeOptions.Value.MinimumMeanScoreForSynoymns)
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
        private async Task<IList<SimilarWord>> GetFilteredSimilarWordsFromWord(string sourceWord)
        {
            var injectWordSoundsLikeOptions = await wordFinderService.GetWordsThatSoundLikeAsync(sourceWord);
            var filtedinjectWordSoundsLikeOptions = injectWordSoundsLikeOptions
                .OrderByDescending(s => s.Score)
                .Take(mergeOptions?.Value?.MaxWordsToSoundLike ?? injectWordSoundsLikeOptions.Count);

            var injectWordSpellsLikeOptions = await wordFinderService.GetWordsToSpellLikeAsync(sourceWord);
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
        public static IEnumerable<string> FindCommonCharacterReplacements(string sourceWord)
        {
            yield return RemoveDoubleLettersFromString(sourceWord);

            yield return sourceWord.Replace("ph", "f");
            yield return sourceWord.Replace("f", "ph");

            yield return sourceWord[0..^1];
        }

        /// <summary>
        /// Removes double letters from a string to allow easier word comparison.
        /// E.g. Glass => Glas
        /// </summary>
        /// <param name="doubleLetterWord">Word to remove double letters (if any) from.</param>
        /// <returns>Processed word.</returns>
        public static string RemoveDoubleLettersFromString(string doubleLetterWord)
        {
            var characters = doubleLetterWord.ToCharArray();
            char lastChar = characters[0];
            string singleLetterWord = lastChar.ToString();

            foreach (char letter in characters)
            {
                if (letter != lastChar)
                {
                    singleLetterWord += letter;
                    lastChar = letter;
                }
            }

            return singleLetterWord;
        }
    }
}
