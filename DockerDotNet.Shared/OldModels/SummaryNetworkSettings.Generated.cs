using System.Collections.Generic;
using System.Runtime.Serialization;

using DockerDotNet.Shared.Models;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class SummaryNetworkSettings // (types.SummaryNetworkSettings)
    {
        [DataMember(Name = "Networks", EmitDefaultValue = false)]
        public IDictionary<string, EndpointSettings> Networks { get; set; }
    }
}
