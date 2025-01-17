using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Models
{
    public class AuthConfig
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("auth")]
        public string Auth { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("serveraddress")]
        public string ServerAddress { get; set; }

        [JsonProperty("identitytoken")]
        public string IdentityToken { get; set; }

        [JsonProperty("registrytoken")]
        public string RegistryToken { get; set; }
    }
}
