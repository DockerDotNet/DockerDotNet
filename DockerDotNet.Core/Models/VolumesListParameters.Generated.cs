using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class VolumesListParameters // (main.VolumesListParameters)
    {
        //[QueryStringParameter("filters", false, typeof(MapQueryStringConverter))]
        [JsonPropertyName("filters")]
        public IDictionary<string, IDictionary<string, bool>> Filters { get; set; }
    }
}
