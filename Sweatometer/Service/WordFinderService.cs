﻿using System;
using System.Collections;
using System.Collections.Generic;
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
                logger.LogDebug("Exception occured when attempting to hit endpoint (" + apiEndpoint + "):" + exception.ToString());
            }

            return similarWords;
        }

        ///<inheritdoc/>
        public async Task<ICollection<SimilarWord>> MergeWords(string fixedWord, string pivotWord)
        {
            var mappedPairs = new List<SimilarWord>();
            var pivotWordOptions = await GetWordsThatSoundLikeAsync(pivotWord);

            foreach(var similarWordOption in pivotWordOptions)
            {
                if (fixedWord.Contains(similarWordOption.Word))
                {
                    var replaceStartIndex = fixedWord.IndexOf(similarWordOption.Word, StringComparison.Ordinal);
                    var replaceEndIndex = replaceStartIndex + similarWordOption.Word.Length;

                    var mappedPair = new SimilarWord();

                    mappedPair.Word = fixedWord.Substring(0, replaceStartIndex)
                        + pivotWord
                        + fixedWord.Substring(replaceEndIndex);
                    mappedPair.Score = similarWordOption.Score;

                    mappedPairs.Add(mappedPair);
                }
            }
            
            return mappedPairs;
        }
    }
}
