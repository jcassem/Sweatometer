using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sweatometer.Model;
using Sweatometer.Service;

namespace Sweatometer
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordFinderController : Controller
    {
        private readonly ILogger<WordFinderController> logger;

        private readonly IWordFinderService wordFinderService;

        public WordFinderController(
            ILogger<WordFinderController> logger,
            IWordFinderService wordFinderService)
        {
            this.logger = logger;
            this.wordFinderService = wordFinderService;
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

        [HttpGet("merge/{firstWord}/{secondWord}")]
        public async Task<IEnumerable<SimilarWord>> FindMerge(string firstWord, string secondWord)
        {
            logger.LogInformation("Find merge words...");

            var result = await wordFinderService.MergeWords(firstWord, secondWord);

            return result.ToArray();
        }
    }
}
