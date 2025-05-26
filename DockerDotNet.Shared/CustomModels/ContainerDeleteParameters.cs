using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Shared.Models
{
    public class ContainerDeleteParameters
    {
        [JsonPropertyName("v")]
        public bool? IncludeVolumes { get; set; }

        [JsonPropertyName("force")]
        public bool? Force { get; set; }

        [JsonPropertyName("link")]
        public bool? Link { get; set; }
    }
}
