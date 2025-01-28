using Newtonsoft.Json;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class ImagesListParameters // (main.ImagesListParameters)
    {
        //[QueryStringParameter("all", false, typeof(BoolQueryStringConverter))]
        [JsonProperty("all")]
        public bool? All { get; set; }

        //[QueryStringParameter("filters", false, typeof(MapQueryStringConverter))]
        [JsonProperty("filters")]
        public IDictionary<string, IDictionary<string, bool>> Filters { get; set; }

        //[QueryStringParameter("digests", false, typeof(BoolQueryStringConverter))]
        [JsonProperty("digests")]
        public bool? Digests { get; set; }

        //[QueryStringParameter("shared-size", false, typeof(BoolQueryStringConverter))]
        [JsonProperty("shared-size")]
        public bool? SharedSize { get; set; }
    }
}
