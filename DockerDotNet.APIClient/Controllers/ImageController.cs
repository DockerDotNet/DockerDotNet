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

        public ImageController(DockerClient dockerClient, ImageService imageService)
        {
            _dockerClient = dockerClient;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllImages([FromQuery] ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            var response = await _imageService.GetImages(imagesListParameters, cancellationToken);   
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(result)
                );
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetImage(string name, CancellationToken cancellationToken)
        {
            var response = await _imageService.GetImage(name, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpGet]
        [Route("{name}/history")]
        public async Task<IActionResult> GetImageHistory(string name, CancellationToken cancellationToken)
        {
            var response = await _imageService.GetImageHistory(name, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpPost]
        [Route("{name}/tag")]
        public async Task<IActionResult> TagImage(string name, ImageTagParameters imageTagParameters, CancellationToken cancellationToken)
        {
            var response = await _imageService.TagImage(name, imageTagParameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpGet]
        [Route("create")]
        public async System.Threading.Tasks.Task CreateImage([FromQuery] ImagesCreateParameters imagesCreateParameters, CancellationToken cancellationToken)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Expected a WebSocket request.", cancellationToken: cancellationToken);
                return;
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            await _imageService.CreateImage(imagesCreateParameters, webSocket, cancellationToken);
        }

        [HttpPost]
        [Route("build/prune")]
        public async Task<IActionResult> DeleteBuilderCache([FromQuery]BuildPruneParameters parameters, CancellationToken cancellationToken)
        {
            var response = await _imageService.BuildPrune(parameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> DeleteImage(string name, [FromQuery]ImageDeleteParameters parameters, CancellationToken cancellationToken)
        {
            var response = await _imageService.DeleteImage(name, parameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }
    }
}
