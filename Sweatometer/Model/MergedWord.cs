using System;
using System.Text;
using Newtonsoft.Json;

namespace Sweatometer.Model
{
    public class MergedWord : SimilarWord
    {
        public string ParentWord { get; set; }

        public string InjectedWord { get; set; }

        public override bool Equals(object obj)
        {
            return obj is MergedWord word &&
                   base.Equals(obj) &&
                   ParentWord == word.ParentWord &&
                   InjectedWord == word.InjectedWord;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), ParentWord, InjectedWord);
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append($"{base.ToString()} ")
                .Append($"{nameof(ParentWord)}: {ParentWord} ")
                .Append($"{nameof(InjectedWord)}: {InjectedWord}")
                .ToString();
        }
    }
}
