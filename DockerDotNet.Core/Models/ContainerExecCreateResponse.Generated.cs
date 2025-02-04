using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class ContainerExecCreateResponse // (main.ContainerExecCreateResponse)
    {
        [DataMember(Name = "Id", EmitDefaultValue = false)]
        public string ID { get; set; }
    }
}
