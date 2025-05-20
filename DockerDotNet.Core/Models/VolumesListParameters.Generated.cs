using Newtonsoft.Json;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class VolumesListParameters // (main.VolumesListParameters)
    {
        //[QueryStringParameter("filters", false, typeof(MapQueryStringConverter))]
        [JsonProperty("filters")]
        public IDictionary<string, IDictionary<string, bool>> Filters { get; set; }
    }
}
