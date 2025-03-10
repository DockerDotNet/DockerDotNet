using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class HealthConfig // (container.HealthConfig)
    {
        [JsonPropertyName("Test")]
        public IList<string> Test { get; set; }

        [JsonPropertyName("Interval")]
        // TODO: See how to handle this converter
        //[JsonConverter(typeof(TimeSpanNanosecondsConverter))]
        public TimeSpan Interval { get; set; }

        [JsonPropertyName("Timeout")]
        // TODO: See how to handle this converter
        //[JsonConverter(typeof(TimeSpanNanosecondsConverter))]
        public TimeSpan Timeout { get; set; }

        [JsonPropertyName("StartPeriod")]
        public long StartPeriod { get; set; }

        [JsonPropertyName("Retries")]
        public long Retries { get; set; }
    }
}
