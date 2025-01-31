using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class TopologyRequirement // (volume.TopologyRequirement)
    {
        [JsonPropertyName("Requisite")]
        public IList<VolumeTopology> Requisite { get; set; }

        [JsonPropertyName("Preferred")]
        public IList<VolumeTopology> Preferred { get; set; }
    }
}
