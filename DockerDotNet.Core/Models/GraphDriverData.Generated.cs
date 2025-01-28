using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DockerDotNet.Core.Models
{
    [DataContract]
    public class GraphDriverData // (types.GraphDriverData)
    {
        [DataMember(Name = "Data", EmitDefaultValue = false)]
        public IDictionary<string, string> Data { get; set; }

        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string Name { get; set; }
    }
}
