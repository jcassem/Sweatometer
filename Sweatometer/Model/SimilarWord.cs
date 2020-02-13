using Newtonsoft.Json;

namespace Sweatometer.Model
{
    public class SimilarWord
    {
        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("numSyllables")]
        public long NumSyllables { get; set; }

        public SimilarWord() { }

        public SimilarWord(string Word, long Score, long NumSyllables = -1)
        {
            this.Word = Word;
            this.Score = Score;
            this.NumSyllables = NumSyllables;
        }
    }
}
