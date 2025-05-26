using DockerDotNet.Core;
using DockerDotNet.Core.Helpers;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

using Microsoft.AspNetCore.Mvc;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/exec")]
    [ApiController]
    public class ExecController : ControllerBase
    {
        private readonly ExecService execService;
        private readonly StreamHelper streamHelper;

        private DockerClient DockerClient { get; set; }

        public ExecController(DockerClient dockerClient, ExecService execService, StreamHelper streamHelper)
        {
            DockerClient = dockerClient;
            this.execService = execService;
            this.streamHelper = streamHelper;
        }

        [HttpGet]
        [Route("{id}/inspect")]
        public async Task<ActionResult> InspectExecInstance(string id, CancellationToken cancellationToken)
        {
            var response = await execService.InspectExec(id, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: exec => Ok(exec)
                );
        }


        [HttpPost]
        [Route("{id}/create")]
        //[ProducesDefaultResponseType(typeof(ContainerExecCreateResponse))]
        public async Task<IActionResult> CreateExecInstance(string id, [FromBody]Shared.Models.ExecConfig execConfig, CancellationToken cancellationToken)
        {
            var response = await execService.CreateExec(id, execConfig, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: exec => Ok(exec)
                );
        }

        [HttpGet]
        [Route("{id}/start")]
        public async Task<IActionResult> StartExecInstance(string id, [FromQuery]ExecStartConfig parameters, CancellationToken cancellationToken)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Expected a WebSocket request.", cancellationToken: cancellationToken);
                return StatusCode(500);
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var response = await execService.StartExecInstance(id, parameters, webSocket, cancellationToken);

            if (response.IsLeft)
            {
                var error = response.Match(Left: error => error, Right: _ => null);
                
                return StatusCode((int)error.StatusCode,error.Message);
            }

            var stream = response.Match(Left: _ => null, Right: str=> str);

            await streamHelper.HandleMultiplexedStreamAsync(parameters.Tty.GetValueOrDefault(), stream,webSocket, cancellationToken);

            return Ok();

        }

        [HttpPost]
        [Route("{id}/resize")]
        public async Task<IActionResult> ResizeExecInstance(string id, [FromQuery]int height, [FromQuery]int width, CancellationToken cancellationToken)
        {
            var response = await execService.ReizeExec(id, height, width, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: exec => Ok(exec)
                );
        }
    }
}
