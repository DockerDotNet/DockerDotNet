using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class WeightDevice // (blkiodev.WeightDevice)
    {
        [JsonPropertyName("Path")]
        public string Path { get; set; }

        [JsonPropertyName("Weight")]
        public ushort Weight { get; set; }
    }
}
