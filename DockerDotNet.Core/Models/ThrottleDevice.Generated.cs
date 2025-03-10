using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class ThrottleDevice // (blkiodev.ThrottleDevice)
    {
        [JsonPropertyName("Path")]
        public string Path { get; set; }

        [JsonPropertyName("Rate")]
        public ulong Rate { get; set; }
    }
}
