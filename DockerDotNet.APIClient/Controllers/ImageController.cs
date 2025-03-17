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
        public async Task<IActionResult> GetAllImages([FromQuery] ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            var(success, response, error) = await _imageService.GetImages(imagesListParameters, cancellationToken);   
            return success ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetImage(string name, CancellationToken cancellationToken)
        {
            var(success, response, error) = await _imageService.GetImage(name, cancellationToken);
            return success ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpGet]
        [Route("{name}/history")]
        public async Task<IActionResult> GetImageHistory(string name, CancellationToken cancellationToken)
        {
            var (success, response, error) = await _imageService.GetImageHistory(name, cancellationToken);
            return success ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpPost]
        [Route("{name}/tag")]
        public async Task<IActionResult> TagImage(string name, ImageTagParameters imageTagParameters, CancellationToken cancellationToken)
        {
            var (success, response, error) = await _imageService.TagImage(name, imageTagParameters, cancellationToken);
            return success ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpPost]
        [Route("pull")]
        public async System.Threading.Tasks.Task PullImage([FromQuery] ImagesCreateParameters imagesCreateParameters, [FromBody] Stream image, CancellationToken cancellationToken)
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
