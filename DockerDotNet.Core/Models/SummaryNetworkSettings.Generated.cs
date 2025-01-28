using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class SummaryNetworkSettings // (types.SummaryNetworkSettings)
    {
        [DataMember(Name = "Networks", EmitDefaultValue = false)]
        public IDictionary<string, EndpointSettings> Networks { get; set; }
    }
}
