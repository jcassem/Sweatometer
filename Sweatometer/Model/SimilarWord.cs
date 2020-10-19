using System;
using System.Text;
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

        public SimilarWordType Type {get; set;}

        public SimilarWord() { }

        public SimilarWord(string Word, long Score, long NumSyllables = -1)
        {
            this.Word = Word;
            this.Score = Score;
            this.NumSyllables = NumSyllables;
        }

        public override bool Equals(object obj)
        {
            return obj is SimilarWord word &&
                   Word == word.Word &&
                   Score == word.Score &&
                   NumSyllables == word.NumSyllables;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Word, Score, NumSyllables);
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append($"{nameof(Word)}: {Word} ")
                .Append($"{nameof(Score)}: {Score} ")
                .Append($"{nameof(NumSyllables)}: {NumSyllables}")
                .ToString();
        }
    }

    public enum SimilarWordType {
        SOUNDS_LIKE,
        MEANS_LIKE,
        SPELLS_LIKE,
        RELATED
    }
}
