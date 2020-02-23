using System.Collections.Generic;
using System.Threading.Tasks;
using Sweatometer.Model;

namespace Sweatometer.Service
{
    /// <summary>
    /// Provides services to merge words.
    /// </summary>
    public interface ISweatTestService
    {
        /// <summary>
        /// Produces a numerical score /100 of the sweatiness of the provided word combination.
        /// </summary>
        /// <param name="parentWord">Word to inject into.</param>
        /// <param name="injectWord">Word to inject.</param>
        /// <param name="providedAnswer">Provided answer.</param>
        /// <returns>Sweat level /100.</returns>
        Task<SweatTestResult> SweatTest(string parentWord, string injectWord, string providedAnswer);
    }
}
