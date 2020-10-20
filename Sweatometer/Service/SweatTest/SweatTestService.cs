using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sweatometer.Model;
using Sweatometer.Service;
using Sweatometer.Service.Merge;

namespace Sweatometer.Service.SweatTest {
    /// <inheritdoc/>
    public class SweatTestService : ISweatTestService {
        private readonly ILogger<SweatTestService> logger;

        private readonly IMergeService mergeService;

        public SweatTestService(
            ILogger<SweatTestService> logger,
            IMergeService mergeService) {
            this.logger = logger;
            this.mergeService = mergeService;
        }

        /// <inheritdoc/>
        public async Task<SweatTestResult> SweatTest(string parentWord, string injectWord, string providedAnswer) {
            providedAnswer = providedAnswer.ToLower();
            SweatTestResult result = new SweatTestResult();

            var foundOptions = await mergeService.MergeWords(
                parentWord,
                injectWord,
                ResultSet.ALL_RESULTS,
                SynonymsOfInjectWord.INCLUDE,
                SynonymsOfParentWord.EXCLUDE
            );

            NormaliseScores(foundOptions);

            result.Alternatives = foundOptions.Where(x => x.Word != providedAnswer).ToList();
            var foundAnswers = foundOptions.Where(x => x.Word == providedAnswer);

            if (foundAnswers?.Count() > 0) {
                result.Score = foundAnswers.First().Score;
                result.Outcome = DetermineSweatLevels(foundOptions, foundAnswers.First());
            }

            logger.LogInformation("SweatTest of '" + providedAnswer + "' has score: " + result.Score);

            return result;
        }

        /// <summary>
        /// Evaluate the proposed pun solution in relation to the others to determine its sweat level.
        /// </summary>
        /// <param name="options">All merge word options.</param>
        /// <param name="candidate">Proposed pun.</param>
        /// <returns>Sweat level string.</returns>
        private string DetermineSweatLevels(ICollection<MergedWord> options, MergedWord candidate) {
            if (options.Contains(candidate)) {
                var denominator = options.Select(x => x.Score).Max();
                var ratio = denominator == 0 ? 0 : candidate.Score / denominator;

                if (ratio >= 0.9) {
                    return SweatTestResult.LOW_SWEAT;
                }
                else if (ratio >= 0.7) {
                    return SweatTestResult.AVERAGE_SWEAT;
                }
                else if (ratio >= 0.5) {
                    return SweatTestResult.PRETTY_SWEATY;
                }
                else {
                    return SweatTestResult.SUPER_SWEATY;
                }

            }

            return SweatTestResult.UNKNOWN;
        }

        /// <summary>
        /// Normalises scores for a collection of merged words to be within a range of 100 providing
        /// the collection has more than on element in it and scores are already > 100.
        /// </summary>
        /// <param name="mergedWordsToNormalise">Collection of MergeWords to normalise scores for.</param>
        private void NormaliseScores(ICollection<MergedWord> mergedWordsToNormalise) {
            if (mergedWordsToNormalise?.Count > 1) {
                var maxScore = mergedWordsToNormalise.Select(x => x.Score).Max();

                if (maxScore > 100) {
                    foreach (var mergedWord in mergedWordsToNormalise) {
                        mergedWord.Score /= maxScore * 100;
                    }
                }
            }
        }
    }
}