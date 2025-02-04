using DockerDotNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        public SystemController()
        {
            DockerClient = new DockerClient();
        }

        [HttpGet]
        [Route("version")]
        public async Task<string> GetVersion()
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new UriBuilder($"{httpClient.BaseAddress}version").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}
