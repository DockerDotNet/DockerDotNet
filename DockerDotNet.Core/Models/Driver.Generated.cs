using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class Driver // (mount.Driver)
    {
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Name = "Options", EmitDefaultValue = false)]
        public IDictionary<string, string> Options { get; set; }
    }
}
