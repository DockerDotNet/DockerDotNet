using DockerDotNet.Core.Models;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class ImagesCreateParameters // (main.ImagesCreateParameters)
    {
        //[QueryStringParameter("fromImage", false)]
        [JsonPropertyName("fromImage")]
        public string FromImage { get; set; }

        //[QueryStringParameter("fromSrc", false)]
        [JsonPropertyName("fromSrc")]
        public string FromSrc { get; set; }

        //[QueryStringParameter("repo", false)]
        [JsonPropertyName("repo")]
        public string Repo { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("changes")]
        public IList<string> Changes { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonIgnore]
        public AuthConfig RegistryAuth { get; set; }
    }
}
