using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Models
{
    public class NetworkInspectParameters
    {
        [JsonPropertyName("verbose")]
        public bool? Verbose { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }
}
