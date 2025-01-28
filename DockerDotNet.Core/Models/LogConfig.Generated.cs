using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class LogConfig // (container.LogConfig)
    {
        [DataMember(Name = "Type", EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(Name = "Config", EmitDefaultValue = false)]
        public IDictionary<string, string> Config { get; set; }
    }
}
