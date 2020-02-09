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
    [Route("[controller]")]
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

        [HttpGet]
        public async Task<IEnumerable<SimilarWord>> Get()
        {
            logger.LogInformation("Get similar words to...");

            var result = await wordFinderService.GetWordsThatSoundLikeAsync("test");

            return result.ToArray();
        }
    }
}
