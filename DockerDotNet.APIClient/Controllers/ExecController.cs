using DockerDotNet.Core;
using DockerDotNet.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;

using System.Text.Json;

namespace Models.Core.Models.APIClient.Controllers
{
    [Route("api/exec")]
    [ApiController]
    public class ExecController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        public ExecController()
        {
            DockerClient = new DockerClient();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ContainerExecInspectResponse> InspectExecInstance(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, $"exec/{id}/json", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            ContainerExecInspectResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerExecInspectResponse>(jsonSerializerOptions, cancellationToken);
            
            return responseContent;
        }

    }
}
