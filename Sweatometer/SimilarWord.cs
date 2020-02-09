using Newtonsoft.Json;

namespace Sweatometer
{
    public class SimilarWord
    {
        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("numSyllables")]
        public long NumSyllables { get; set; }
    }
}
