using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sweatometer.Service;

namespace Sweatometer
{
    public class WordFinderController : Controller
    {
        private readonly ILogger<WordFinderController> _logger;

        private readonly IWordFinderService wordFinderService;

        public WordFinderController(
            ILogger<WordFinderController> logger,
            IWordFinderService wordFinderService)
        {
            _logger = logger;
            this.wordFinderService = wordFinderService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> IndexAsync()
        {
            var result = await wordFinderService.GetWordsThatSoundLikeAsync("test");

            return View();
        }
    }
}
