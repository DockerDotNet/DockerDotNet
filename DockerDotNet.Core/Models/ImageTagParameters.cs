using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Models
{
    public class ImageTagParameters
    {
        [JsonPropertyName("repo")]
        public string Repository { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

    }
}
