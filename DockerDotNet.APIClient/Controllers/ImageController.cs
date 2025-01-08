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
                var headers = GetAuthHeaders();
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


        private Dictionary<string, string> GetAuthHeaders()
        {
            string authString = """{"username":"excellonb2bregsrv","password":"bo3NnGf9UhG=iRFSSoK51Xrlemm5CNUv","serveraddress":"excellonb2bregsrv.azurecr.io"}""";
                //"username": "aniketmespl",
                //"password": "Excellon@123",
                //"email": "aniketm@excellonsoft.com",
                //"serveraddress": "https://index.docker.io/v1/"

            return new Dictionary<string, string>
            {
                {
                    "X-Registry-Auth",
                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authString))
                    .Replace("/", "_").Replace("+", "-") 
                    // This is not documented in Docker API but from source code (https://github.com/docker/docker-ce/blob/10e40bd1548f69354a803a15fde1b672cc024b91/components/cli/cli/command/registry.go#L47)
                    // and from multiple internet sources it has to be base64-url-safe. 
                    // See RFC 4648 Section 5. Padding (=) needs to be kept.
                }
            };
        }
    }
}
