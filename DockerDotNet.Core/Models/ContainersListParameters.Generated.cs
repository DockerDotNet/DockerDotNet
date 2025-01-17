using Newtonsoft.Json;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Docker.DotNet.Models
{
    [DataContract]
    public class ContainersListParameters // (main.ContainersListParameters)
    {
        //[QueryStringParameter("size", false, typeof(BoolQueryStringConverter))]
        [JsonProperty("size")]
        public bool? Size { get; set; }

        [JsonProperty("all")]
        //[QueryStringParameter("all", false, typeof(BoolQueryStringConverter))]
        public bool? All { get; set; }

        //[QueryStringParameter("since", false)]
        //public string Since { get; set; }

        //[QueryStringParameter("before", false)]
        //public string Before { get; set; }

        //[QueryStringParameter("limit", false)]
        [JsonProperty("limit")]
        public long? Limit { get; set; }

        //[QueryStringParameter("filters", false, typeof(MapQueryStringConverter))]
        [JsonProperty("filters")]
        public IDictionary<string, IDictionary<string, bool>>? Filters { get; set; }
    }
}
