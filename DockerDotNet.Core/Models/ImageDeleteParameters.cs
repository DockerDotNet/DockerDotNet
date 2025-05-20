using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Models
{
    public class ImageDeleteParameters
    {
        [JsonPropertyName("force")]
        public bool? Force { get; set; }

        [JsonPropertyName("noprune")]
        public bool? NoPrune { get; set; }
    }
}
