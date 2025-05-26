using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Shared.Models
{
    [DataContract]
    public class ContainersListParameters // (main.ContainersListParameters)
    {
        //[QueryStringParameter("size", false, typeof(BoolQueryStringConverter))]
        [JsonPropertyName("size")]
        public bool? Size { get; set; }

        [JsonPropertyName("all")]
        //[QueryStringParameter("all", false, typeof(BoolQueryStringConverter))]
        public bool? All { get; set; }

        //[QueryStringParameter("since", false)]
        //public string Since { get; set; }

        //[QueryStringParameter("before", false)]
        //public string Before { get; set; }

        //[QueryStringParameter("limit", false)]
        [JsonPropertyName("limit")]
        public long? Limit { get; set; }

        //[QueryStringParameter("filters", false, typeof(MapQueryStringConverter))]
        [JsonPropertyName("filters")]
        public IDictionary<string, IDictionary<string, bool>>? Filters { get; set; }
    }
}
