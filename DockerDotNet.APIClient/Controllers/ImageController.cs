using DockerDotNet.Shared.Helpers;
using DockerDotNet.Shared.Interfaces;
using DockerDotNet.Shared.Models;

using Microsoft.AspNetCore.Mvc;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly StreamHelper streamHelper;

        public ImageController(IImageService imageService, StreamHelper streamHelper)
        {
            _imageService = imageService;
            this.streamHelper = streamHelper;
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
        public async Task<IActionResult> CreateImage([FromQuery] ImagesCreateParameters imagesCreateParameters, CancellationToken cancellationToken)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Expected a WebSocket request.", cancellationToken: cancellationToken);
                return StatusCode(500);
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var response = await _imageService.CreateImage(imagesCreateParameters, webSocket, cancellationToken);
            if (response.IsLeft)
            {
                return response.Match(Left: error => StatusCode((int)error!.StatusCode,error.Message), Right: _ => null );
            }
            var stream = response.Match(Left: error => null, Right: str => str);

            await streamHelper.HandleNDJsonStreamAsync<CreateImageInfo>(stream, webSocket, cancellationToken);

            return Ok();
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

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchImages([FromQuery]ImageSearchParameters parameters, CancellationToken cancellationToken)
        {
            var response = await _imageService.ImageSearch(parameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }
    }
}
