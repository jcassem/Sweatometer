using System;
using System.Text;

namespace Sweatometer.Model {
    public class Emoji {
        public string Icon { get; set; }

        public string Description { get; set; }

        public Emoji(string Icon, string Description) {
            this.Icon = Icon;
            this.Description = Description;
        }

        public override bool Equals(object obj) {
            return obj is Emoji emoji &&
                base.Equals(obj) &&
                Icon == emoji.Icon &&
                Description == emoji.Description;
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Icon, Description);
        }

        public override string ToString() {
            return new StringBuilder()
                .Append($"{base.ToString()} ")
                .Append($"{nameof(Icon)}: {Icon} ")
                .Append($"{nameof(Description)}: {Description}")
                .ToString();
        }
    }
}