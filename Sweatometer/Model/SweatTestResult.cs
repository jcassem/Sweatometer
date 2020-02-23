
using System.Collections.Generic;
using Sweatometer.Model;

namespace Sweatometer
{
    public class SweatTestResult
    {
        public static readonly string SUPER_SWEATY = "Super Sweaty";
        public static readonly string PRETTY_SWEATY = "Pretty Sweaty";
        public static readonly string AVERAGE_SWEAT = "Average Sweat";
        public static readonly string LOW_SWEAT = "Low Sweat";
        public static readonly string UNKNOWN= "Unknown";

        public string Outcome { get; set; } = UNKNOWN.ToString();

        public long Score { get; set; }

        public ICollection<MergedWord> Alternatives { get; set; }
    }
}
