using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Models
{
    public class ImageSearchParameters
    {
        [JsonRequired]
        [JsonPropertyName("term")]
        public string Term { get; set; }

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("filters")]
        public IDictionary<string, IDictionary<string, bool>>? Filters { get; set; }
    }
}
