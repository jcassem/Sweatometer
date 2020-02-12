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

        [HttpGet("{wordToSoundLike}")]
        public async Task<IEnumerable<SimilarWord>> FindSimilar(string wordToSoundLike)
        {
            logger.LogInformation("Get similar words to...");

            var result = await wordFinderService.GetWordsThatSoundLikeAsync(wordToSoundLike);

            return result.ToArray();
        }

        [HttpGet("{firstWord}/{secondWord}")]
        public async Task<IEnumerable<SimilarWord>> FindMerge(string firstWord, string secondWord)
        {
            logger.LogInformation("Find merge words...");

            var result = await wordFinderService.MergeWords(firstWord, secondWord);

            return result.ToArray();
        }
    }
}
