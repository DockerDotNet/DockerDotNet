using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Shared.Models
{
    public class VolumePruneParameters
    {
        [JsonPropertyName("filters")]
        public IDictionary<string, IDictionary<string, bool>>? Filters { get; set; }
    }
}
