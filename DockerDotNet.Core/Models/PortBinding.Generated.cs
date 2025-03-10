using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class PortBinding // (nat.PortBinding)
    {
        [JsonPropertyName("HostIp")]
        public string HostIP { get; set; }

        [JsonPropertyName("HostPort")]
        public string HostPort { get; set; }
    }
}
