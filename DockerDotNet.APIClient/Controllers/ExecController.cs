using DockerDotNet.Core;
using DockerDotNet.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;

using System.Text.Json;
using DockerDotNet.Core.Services;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/exec")]
    [ApiController]
    public class ExecController : ControllerBase
    {
        private readonly ExecService execService;

        DockerClient DockerClient { get; set; }

        public ExecController(DockerClient dockerClient, ExecService execService)
        {
            DockerClient = dockerClient;
            this.execService = execService;
        }

        [HttpPost]
        [Route("{id}")]
        //[ProducesDefaultResponseType(typeof(ContainerExecCreateResponse))]
        public async Task<IActionResult> CreateExecInstance(string id, [FromBody]ExecConfig execConfig, CancellationToken cancellationToken)
        {
            var response = await execService.CreateExec(id, execConfig, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: exec => Ok(exec)
                );
        }

        [HttpGet]
        [Route("{id}/start")]
        public async System.Threading.Tasks.Task StartExecInstance(string id, [FromQuery]ExecStartConfig parameters, CancellationToken cancellationToken)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Expected a WebSocket request.", cancellationToken: cancellationToken);
                return;
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var (success, stream, error) = await execService.StartExecInstance(id, parameters, webSocket, cancellationToken);

        }
    }
}
