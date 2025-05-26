

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Shared.Models
{
    public class ContainerInspectParameters // (main.ContainerInspectParameters)
    {
        //[QueryStringParameter("size", false, typeof(BoolQueryStringConverter))]
        [JsonPropertyName("size")]
        public bool? IncludeSize { get; set; }
    }
}
