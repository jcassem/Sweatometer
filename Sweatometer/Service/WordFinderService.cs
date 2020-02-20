using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sweatometer.Model;

namespace Sweatometer.Service
{
    ///<inheritdoc/>
    public class WordFinderService : IWordFinderService
    {
        public static readonly string DATAMUSE_SOUNDS_LIKE_API = "https://api.datamuse.com/words?sl=";

        public static readonly string DATAMUSE_SPELLS_LIKE_API = "https://api.datamuse.com/words?sp=";

        public static readonly string DATAMUSE_MEANS_LIKE_API = "https://api.datamuse.com/words?ml=";

        public static readonly string DATAMUSE_SUGGEST_API = "https://api.datamuse.com/sug?s=";

        private readonly ILogger<WordFinderService> logger;

        public WordFinderService(ILogger<WordFinderService> logger)
        {
            this.logger = logger;
        }

        ///<inheritdoc/>
        public async Task<ICollection<SimilarWord>> GetWordsThatSoundLikeAsync(string wordToSoundLike)
        {
            return await GetWordsFromDataMuseApi(DATAMUSE_SOUNDS_LIKE_API + wordToSoundLike);
        }

        ///<inheritdoc/>
        public async Task<ICollection<SimilarWord>> GetWordsToMeanLikeAsync(string wordToMeanLike)
        {
            return await GetWordsFromDataMuseApi(DATAMUSE_MEANS_LIKE_API + wordToMeanLike);
        }

        ///<inheritdoc/>
        public async Task<ICollection<SimilarWord>> GetWordsToSpellLikeAsync(string wordToSpellLike)
        {
            return await GetWordsFromDataMuseApi(DATAMUSE_SPELLS_LIKE_API + wordToSpellLike);
        }

        ///<inheritdoc/>
        public async Task<ICollection<SimilarWord>> GetSuggestedWordsAsync(string toSuggestFrom)
        {
            return await GetWordsFromDataMuseApi(DATAMUSE_SUGGEST_API + toSuggestFrom);
        }

        /// <summary>
        /// Sends a request to the DataMuse endpoint provided and returns the collection of similar words provided.
        /// </summary>
        /// <param name="apiEndpoint">DataMuse endpoint to send a request to.</param>
        /// <returns>Collection of similar words.</returns>
        public async Task<ICollection<SimilarWord>> GetWordsFromDataMuseApi(string apiEndpoint)
        {
            var similarWords = new List<SimilarWord>();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(apiEndpoint))
                    {
                        using (HttpContent content = response.Content)
                        {
                            string data = await content.ReadAsStringAsync();

                            if (data != null)
                            {
                                similarWords = JsonConvert.DeserializeObject<List<SimilarWord>>(data);
                            }
                            else
                            {
                                //If data is null log it into console.
                                logger.LogDebug("Data is null from request url: " + apiEndpoint);
                            }
                        }
                    }
                }
                //Catch any exceptions and log it into the console.
            }
            catch (Exception exception)
            {
                logger.LogDebug(exception, $"Exception occurred when attempting to hit endpoint ({apiEndpoint}).");
            }

            return similarWords;
        }

        ///<inheritdoc/>
        public async Task<ICollection<MergedWord>> MergeWords(string fixedWord, string injectWord)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var mergedWords = await MergeWords(fixedWord, injectWord, MergeWordsOptions.Default);

            watch.Stop();
            logger.LogDebug("MergeWord Runtime: " + watch.ElapsedMilliseconds + " milliseconds"); 

            return mergedWords;
        }

        public async Task<ICollection<MergedWord>> MergeWords(string fixedWord, string injectWord, MergeWordsOptions mergeWordsOptions)
        {
            var mappedPairs = new List<MergedWord>();

            // Create a list of synonyms for the injected word
            var injectWords = new List<string>();
            injectWords.Add(injectWord);
            injectWords.AddRange(await GetFilteredSynonymsOfWord(injectWord, mergeWordsOptions));

            // Go through each inject word option (synonyms) and each of their related words (sounds like, spells like)
            foreach (var selectedinjectWord in injectWords)
            {
                var pivotOptions = await GetFilteredSimilarWordsFromWord(selectedinjectWord, mergeWordsOptions);

                // Process each similar word option and the word replacements for these words.
                foreach (var similarWordOption in pivotOptions)
                {
                    foreach (string wordAttempt in FindCommonCharacterReplacements(similarWordOption.Word)
                        .Where(w => fixedWord.Contains(w)))
                    {
                        var replaceStartIndex = fixedWord.IndexOf(wordAttempt, StringComparison.Ordinal);
                        var replaceEndIndex = replaceStartIndex + wordAttempt.Length;

                        MergedWord match = new MergedWord
                        {
                            Word = fixedWord.Substring(0, replaceStartIndex) + selectedinjectWord + fixedWord.Substring(replaceEndIndex),
                            Score = similarWordOption.Score,
                            InjectedWord = wordAttempt,
                            ParentWord = fixedWord
                        };

                        if (!mappedPairs.Any(x => x.Word == match.Word))
                        {
                            mappedPairs.Add(match);
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
        /// <param name="mergeWordsOptions">Filter options.</param>
        /// <returns>Filtered list of synonyms.</returns>
        private async Task<IEnumerable<string>> GetFilteredSynonymsOfWord(string sourceWord, MergeWordsOptions mergeWordsOptions)
        {
            if (mergeWordsOptions.CheckSynonyms)
            {
                var synonyms = await GetWordsToMeanLikeAsync(sourceWord);

                return synonyms
                    .Where(s => s.Score > mergeWordsOptions.MeansLikeMinimumScore)
                    .OrderByDescending(s => s.Score)
                    .Take(mergeWordsOptions.MaxSynonyms)
                    .Select(s => s.Word)
                    .Distinct();
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Provides a list of alternative word options that sound or spell like the provided one.
        /// </summary>
        /// <param name="sourceWord">Word to search against.</param>
        /// <param name="mergeWordsOptions">Filter options.</param>
        /// <returns>Filtered down list of options based of <code>mergeWordOptions</code></returns>
        private async Task<IList<SimilarWord>> GetFilteredSimilarWordsFromWord(string sourceWord, MergeWordsOptions mergeWordsOptions)
        {
            var injectWordSoundsLikeOptions = await GetWordsThatSoundLikeAsync(sourceWord);
            var filtedinjectWordSoundsLikeOptions = injectWordSoundsLikeOptions
                .OrderByDescending(s => s.Score)
                .Take(mergeWordsOptions.MaxWordsToSoundLike);

            var injectWordSpellsLikeOptions = await GetWordsToSpellLikeAsync(sourceWord);
            var filteredinjectWordSpellsLikeOptions = injectWordSpellsLikeOptions
                .OrderByDescending(s => s.Score)
                .Take(mergeWordsOptions.MaxWordsToSpellLike);

            return filtedinjectWordSoundsLikeOptions.Concat(filteredinjectWordSpellsLikeOptions).ToList();
        }

        /// <summary>
        /// Yields an altered version of the provided word with some characters replaced
        /// to allow for easier word comparision e.g. removing double letters and inter-changing 'ph' and 'f'.
        /// </summary>
        /// <param name="sourceWord">Word to process.</param>
        /// <returns>Processed words with character replacements.</returns>
        private IEnumerable<string> FindCommonCharacterReplacements(string sourceWord)
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
