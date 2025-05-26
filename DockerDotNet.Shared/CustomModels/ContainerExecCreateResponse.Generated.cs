using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Shared.Models
{
    public class ContainerExecCreateResponse // (main.ContainerExecCreateResponse)
    {
        [JsonPropertyName("Id")]
        public string ID { get; set; }
    }
}
