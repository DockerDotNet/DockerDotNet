using DockerDotNet.Core;
using DockerDotNet.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/volumes")]
    [ApiController]
    public class VolumeController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        public VolumeController()
        {
            DockerClient = new DockerClient();
        }

        [HttpGet]
        public async Task<VolumesListResponse> GetVolumes([FromQuery] VolumesListParameters volumesListParameters, CancellationToken cancellationToken)
        {
            string queryString = DockerClient.GetQueryString(volumesListParameters);

            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            //httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, "volumes", queryString);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();

            VolumesListResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<VolumesListResponse>(cancellationToken);
            return responseContent; 
        }
    }
}
