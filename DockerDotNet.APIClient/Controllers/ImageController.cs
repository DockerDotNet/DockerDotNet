using DockerDotNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly DockerClient _dockerClient;

        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _dockerClient = new DockerClient();
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IList<ImagesListResponse>> GetAllImages([FromQuery] ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            return await _imageService.GetImages(imagesListParameters, cancellationToken).ConfigureAwait(false);
        }

        [HttpPost]
        [Route("pull")]
        public async Task PullImage([FromQuery] ImagesCreateParameters imagesCreateParameters, [FromBody] Stream image, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Need to handle conversion of stream to HttpContent
                HttpClient httpClient = _dockerClient.GetDockerHttpClient();
                string parameters = _dockerClient.GetQueryString(imagesCreateParameters);
                //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new UriBuilder($"{httpClient.BaseAddress}images/create?fromImage={imageName}").Uri);
                HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, "images/create", parameters);

                requestMessage.Headers.Add("Accept", "application/json");
                var headers = _dockerClient.GetRegistryAuthHeaders(null);
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
