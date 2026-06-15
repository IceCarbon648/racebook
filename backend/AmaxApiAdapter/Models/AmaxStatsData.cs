using System.Text.Json.Serialization;

namespace AmaxApiAdapter.Models
{
    public class AmaxStatsData
    {
        [JsonPropertyName("statLevel")]
        public int StatLevel { get; set; }

        [JsonPropertyName("statFans")]
        public int StatFans { get; set; }

        [JsonPropertyName("statRaceTime")]
        public long StatRaceTime { get; set; }

        [JsonPropertyName("statDriverScore")]
        public int StatDriverScore { get; set; }

        [JsonPropertyName("statTop3")]
        public int StatTopThree { get; set; }

        [JsonPropertyName("statRaces")]
        public int StatRaces { get; set; }

        [JsonPropertyName("statFirst")]
        public int StatFirst { get; set; }

        [JsonPropertyName("statHits")]
        public int StatHits { get; set; }

        [JsonPropertyName("statFired")]
        public int StatFired { get; set; }

        [JsonPropertyName("statWrecked")]
        public int StatWrecked { get; set; }

        [JsonPropertyName("statLegend")]
        public int StatLegend { get; set; }

        [JsonPropertyName("statLegendTime")]
        public long StatLegendTime { get; set; }
    }
}