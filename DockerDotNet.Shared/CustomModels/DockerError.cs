using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.Shared.Models
{
    public class DockerError
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }

        public DockerError(HttpStatusCode statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
