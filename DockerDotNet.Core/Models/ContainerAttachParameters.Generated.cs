using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class ContainerAttachParameters // (main.ContainerAttachParameters)
    {
        //[QueryStringParameter("stream", false, typeof(BoolQueryStringConverter))]
        [JsonProperty("stream")]
        public bool? Stream { get; set; }

        [JsonProperty("stdin")]
        //[QueryStringParameter("stdin", false, typeof(BoolQueryStringConverter))]
        public bool? Stdin { get; set; }

        [JsonProperty("stdout")]
        //[QueryStringParameter("stdout", false, typeof(BoolQueryStringConverter))]
        public bool? Stdout { get; set; }

        [JsonProperty("stderr")]
        //[QueryStringParameter("stderr", false, typeof(BoolQueryStringConverter))]
        public bool? Stderr { get; set; }

        [JsonProperty("detachKeys")]
        //[QueryStringParameter("detachKeys", false)]
        public string DetachKeys { get; set; }

        [JsonProperty("logs")]
        //[QueryStringParameter("logs", false)]
        public string Logs { get; set; }
    }
}
