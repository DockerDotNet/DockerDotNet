using DockerDotNet.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        public ImageController()
        {
            DockerClient = new DockerClient();
        }

        [HttpPost]
        [Route("pull")]
        public async Task PullImage([FromQuery]string imageName, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient httpClient = DockerClient.GetDockerHttpClient();

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new UriBuilder($"{httpClient.BaseAddress}images/create?fromImage={imageName}").Uri);
                requestMessage.Headers.Add("Accept", "application/json");
                var headers = DockerClient.GetRegistryAuthHeaders(null);
                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);


                Response.StatusCode = (int)httpResponseMessage.StatusCode;
                Response.ContentType = httpResponseMessage.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                using var upstreamStream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
                await upstreamStream.CopyToAsync(Response.Body, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
