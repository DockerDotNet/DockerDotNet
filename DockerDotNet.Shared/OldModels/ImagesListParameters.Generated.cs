using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class ImagesListParameters // (main.ImagesListParameters)
    {
        [JsonPropertyName("all")]
        public bool? All { get; set; }

        [JsonPropertyName("filters")]
        public IDictionary<string, IDictionary<string, bool>> Filters { get; set; }

        [JsonPropertyName("digests")]
        public bool? Digests { get; set; }

        [JsonPropertyName("shared-size")]
        public bool? SharedSize { get; set; }
    }
}
