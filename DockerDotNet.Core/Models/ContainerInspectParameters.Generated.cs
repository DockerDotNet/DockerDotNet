using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class ContainerInspectParameters // (main.ContainerInspectParameters)
    {
        //[QueryStringParameter("size", false, typeof(BoolQueryStringConverter))]
        [JsonProperty("size")]
        public bool? IncludeSize { get; set; }
    }
}
