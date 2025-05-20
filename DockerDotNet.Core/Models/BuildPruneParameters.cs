using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Models
{
    public class BuildPruneParameters
    {
        [JsonPropertyName("keep-storage")]
        public long? KeepStorage { get; set; }

        [JsonPropertyName("reserved-space")]
        public long? ReservedSpace { get; set; }

        [JsonPropertyName("max-used-space")]
        public long? MaxUsedSpace { get; set; }

        [JsonPropertyName("min-free-space")]
        public long? MinFreeSpace { get; set; }

        [JsonPropertyName("all")]
        public bool? All { get; set; }

        [JsonPropertyName("filters")]
        public IDictionary<string, IDictionary<string, bool>>? Filters { get; set; }
    }
}
