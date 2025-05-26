using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Shared.Models
{
    [DataContract]
    public class ContainerCreateParameters
    {
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        //[QueryStringParameter("platform", false)]
        [DataMember(Name = "platform", EmitDefaultValue = false)]
        [JsonPropertyName("platform")]
        public string? Platform { get; set; }

    }

}
