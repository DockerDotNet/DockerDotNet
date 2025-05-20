using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Models
{
    public class ContainerLogsParameters
    {
        [JsonPropertyName("follow")]
        public bool? Follow { get; set; }

        [JsonPropertyName("stdout")]
        public bool? StdOut { get; set; }

        [JsonPropertyName("stderr")]
        public bool? StdErr { get; set; }

        [JsonPropertyName("since")]
        public int? Since { get; set; }

        [JsonPropertyName("until")]
        public int? Until { get; set; }

        [JsonPropertyName("timestamps")]
        public bool? TimeStamps { get; set; }

        [JsonPropertyName("tail")]
        public string? Tail { get; set; }
    }
}
