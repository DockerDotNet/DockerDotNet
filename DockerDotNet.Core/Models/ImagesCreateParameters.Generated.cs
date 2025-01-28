using DockerDotNet.Core.Models;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class ImagesCreateParameters // (main.ImagesCreateParameters)
    {
        //[QueryStringParameter("fromImage", false)]
        [JsonProperty("fromImage")]
        public string FromImage { get; set; }

        //[QueryStringParameter("fromSrc", false)]
        [JsonProperty("fromSrc")]
        public string FromSrc { get; set; }

        //[QueryStringParameter("repo", false)]
        [JsonProperty("repo")]
        public string Repo { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("changes")]
        public IList<string> Changes { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("x-registry-auth")]
        public AuthConfig RegistryAuth { get; set; }
    }
}
