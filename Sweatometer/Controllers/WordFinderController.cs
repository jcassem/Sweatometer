﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sweatometer.Model;
using Sweatometer.Service.EmojiSearch;
using Sweatometer.Service.Merge;
using Sweatometer.Service.SweatTest;
using Sweatometer.Service.Word;

namespace Sweatometer
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordFinderController : Controller
    {
        private readonly ILogger<WordFinderController> logger;

        private readonly IWordFinderService wordFinderService;

        private readonly IMergeService mergeService;

        private readonly ISweatTestService sweatTestService;

        private readonly IEmojiSearchService emojiSearchService;

        public WordFinderController(
            ILogger<WordFinderController> logger,
            IWordFinderService wordFinderService,
            IMergeService mergeService,
            ISweatTestService sweatTestService,
            IEmojiSearchService emojiSearchService)
        {
            this.logger = logger;
            this.wordFinderService = wordFinderService;
            this.mergeService = mergeService;
            this.sweatTestService = sweatTestService;
            this.emojiSearchService = emojiSearchService;
        }

        [HttpGet("find/soundsLike/{wordToSoundLike}")]
        public async Task<IEnumerable<SimilarWord>> FindSoundLike(string wordToSoundLike)
        {
            logger.LogInformation("Find words to sound like: " + wordToSoundLike);

            var result = await wordFinderService.GetWordsThatSoundLikeAsync(wordToSoundLike);

            return result.ToArray();
        }

        [HttpGet("find/spellsLike/{wordToSpellLike}")]
        public async Task<IEnumerable<SimilarWord>> FindSpellsLike(string wordToSpellLike)
        {
            logger.LogInformation("Find words to spell like: " + wordToSpellLike);

            var result = await wordFinderService.GetWordsToSpellLikeAsync(wordToSpellLike);

            return result.ToArray();
        }

        [HttpGet("find/meansLike/{wordToMeanLike}")]
        public async Task<IEnumerable<SimilarWord>> FindMeansLike(string wordToMeanLike)
        {
            logger.LogInformation("Find words to mean like: " + wordToMeanLike);

            var result = await wordFinderService.GetWordsToMeanLikeAsync(wordToMeanLike);

            return result.ToArray();
        }

        [HttpGet("find/relatedTo/{searchWord}")]
        public async Task<IEnumerable<SimilarWord>> FindRelatedTriggeres(string searchWord)
        {
            logger.LogInformation("Find words realted to: " + searchWord);

            var result = await wordFinderService.GetRelatedTriggerWords(searchWord);

            return result.ToArray();
        }

        [HttpGet("merge/{parentWord}/{injectWord}")]
        public async Task<IEnumerable<SimilarWord>> FindMerge(string parentWord, string injectWord)
        {
            logger.LogInformation("Find merged words for '" + parentWord + "' and '" + injectWord + "'");

            var result = await mergeService.MergeWords(parentWord, injectWord);

            return result.ToArray();
        }

        [HttpGet("sweat/{parentWord}/{injectWord}/{providedAnswer}")]
        public async Task<SweatTestResult> FindSweatTest(string parentWord, string injectWord, string providedAnswer)
        {
            logger.LogInformation("Run Sweat test (" + parentWord + " + " + injectWord + " = " + providedAnswer + ")");

            return await sweatTestService.SweatTest(parentWord, injectWord, providedAnswer);
        }


        [HttpGet("find/emoji/{searchTerm}")]
        public async Task<IEnumerable<Emoji>> FindEmojisLike(string searchTerm)
        {
            logger.LogInformation("Find emojis like: " + searchTerm);

            var result =  await emojiSearchService.FindEmojisThatMatch(searchTerm);

            return result.ToArray();
        }
        
        [HttpGet("find/emoji/set/{searchTerm}")]
        public async Task<IDictionary<string, ICollection<Emoji>>> FindEmojiSetLike(string searchTerm)
        {
            logger.LogInformation("Find emoji set like: " + searchTerm);

            var result =  await emojiSearchService.FindSetOfEmojisThatMatch(searchTerm);

            return result;
        }
    }
}
