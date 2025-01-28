using DockerDotNet.Core.Models;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using DockerDotNet.Core.Models;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class RestartPolicy // (container.RestartPolicy)
    {
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public RestartPolicyKind Name { get; set; }

        [DataMember(Name = "MaximumRetryCount", EmitDefaultValue = false)]
        public long MaximumRetryCount { get; set; }
    }
}
