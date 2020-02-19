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
        public async Task<ICollection<MergedWord>> MergeWords(string fixedWord, string pivotWord)
        {
            return await MergeWords(fixedWord, pivotWord, MergeWordsOptions.Default);
        }

        public async Task<ICollection<MergedWord>> MergeWords(string fixedWord, string pivotWord, MergeWordsOptions mergeWordsOptions)
        {
            var mappedPairs = new List<MergedWord>();

            var pivotWords = new List<string>();
            pivotWords.Add(pivotWord);
            if (mergeWordsOptions.CheckSynonyms)
            {
                var synonyms = await GetWordsToMeanLikeAsync(pivotWord);
                var filteredSynonyms = synonyms
                    .Where(s => s.Score > mergeWordsOptions.MinimumScore)
                    .OrderByDescending(s => s.Score)
                    .Take(mergeWordsOptions.MaxSynonyms)
                    .Select(s => s.Word)
                    .Distinct();
                pivotWords.AddRange(filteredSynonyms);
            }

            foreach (var selectedPivotWord in pivotWords)
            {
                var pivotWordSoundsLikeOptions = await GetWordsThatSoundLikeAsync(selectedPivotWord);
                var pivotWordSpellsLikeOptions = await GetWordsToSpellLikeAsync(selectedPivotWord);
                var pivotOptions = pivotWordSoundsLikeOptions.Concat(pivotWordSpellsLikeOptions).ToList();

                foreach (var similarWordOption in pivotOptions)
                {
                    foreach (string wordAttempt in FindCommonCharacterReplacements(similarWordOption.Word).Where(w => fixedWord.Contains(w)))
                    {
                        var replaceStartIndex = fixedWord.IndexOf(wordAttempt, StringComparison.Ordinal);
                        var replaceEndIndex = replaceStartIndex + wordAttempt.Length;

                        MergedWord match = new MergedWord
                        {
                            Word = fixedWord.Substring(0, replaceStartIndex) + selectedPivotWord + fixedWord.Substring(replaceEndIndex),
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

        private IEnumerable<string> FindCommonCharacterReplacements(string sourceWord)
        {

            yield return RemoveDoubleLettersFromString(sourceWord);

            yield return sourceWord.Replace("ph", "f");
            yield return sourceWord.Replace("f", "ph");
        }
    }
}
