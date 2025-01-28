using DockerDotNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DockerDotNet.Core.Models;

namespace Models.Core.Models.APIClient.Controllers
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

        [HttpGet]
        public async Task<IList<ImagesListResponse>> GetAllImages([FromQuery]ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            string parameters = DockerClient.GetQueryString(imagesListParameters);
            //Uri requestUri = new UriBuilder($"{httpClient.BaseAddress}images/json").Uri;
            //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, "images/json", parameters, null);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            IList<ImagesListResponse>? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<IList<ImagesListResponse>>(cancellationToken);
            //string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        [HttpPost]
        [Route("pull")]
        public async Task PullImage([FromQuery]ImagesCreateParameters imagesCreateParameters,[FromBody]Stream image, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Need to handle conversion of stream to HttpContent
                HttpClient httpClient = DockerClient.GetDockerHttpClient();
                string parameters = DockerClient.GetQueryString(imagesCreateParameters);
                //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new UriBuilder($"{httpClient.BaseAddress}images/create?fromImage={imageName}").Uri);
                HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, "images/create", parameters);

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
