using System;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class Meta // (swarm.Meta)
    {
        [JsonPropertyName("Version")]
        public Version Version { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
