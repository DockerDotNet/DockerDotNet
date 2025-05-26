using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Shared.Models
{
    [DataContract]
    public class ContainerAttachParameters // (main.ContainerAttachParameters)
    {
        //[QueryStringParameter("stream", false, typeof(BoolQueryStringConverter))]
        [JsonPropertyName("stream")]
        public bool? Stream { get; set; }

        [JsonPropertyName("stdin")]
        //[QueryStringParameter("stdin", false, typeof(BoolQueryStringConverter))]
        public bool? Stdin { get; set; }

        [JsonPropertyName("stdout")]
        //[QueryStringParameter("stdout", false, typeof(BoolQueryStringConverter))]
        public bool? Stdout { get; set; }

        [JsonPropertyName("stderr")]
        //[QueryStringParameter("stderr", false, typeof(BoolQueryStringConverter))]
        public bool? Stderr { get; set; }

        [JsonPropertyName("detachKeys")]
        //[QueryStringParameter("detachKeys", false)]
        public string DetachKeys { get; set; }

        [JsonPropertyName("logs")]
        //[QueryStringParameter("logs", false)]
        public string Logs { get; set; }
    }
}
