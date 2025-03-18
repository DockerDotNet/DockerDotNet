using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class GraphDriverData // (types.GraphDriverData)
    {
        [JsonPropertyName("Data")]
        public IDictionary<string, string> Data { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }
    }
}
