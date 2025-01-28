using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class Address // (network.Address)
    {
        [DataMember(Name = "Addr", EmitDefaultValue = false)]
        public string Addr { get; set; }

        [DataMember(Name = "PrefixLen", EmitDefaultValue = false)]
        public long PrefixLen { get; set; }
    }
}
