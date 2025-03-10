

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public class ContainerInspectParameters // (main.ContainerInspectParameters)
    {
        //[QueryStringParameter("size", false, typeof(BoolQueryStringConverter))]
        [JsonPropertyName("size")]
        public bool? IncludeSize { get; set; }
    }
}
