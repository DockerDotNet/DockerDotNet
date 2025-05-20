using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class VolumeTopology // (volume.Topology)
    {
        [JsonPropertyName("Segments")]
        public IDictionary<string, string> Segments { get; set; }
    }
}
