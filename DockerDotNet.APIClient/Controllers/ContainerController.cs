using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using DockerDotNet.Core;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.WebSockets;
using System.Text;
using LanguageExt.Pipes;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/containers")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        private readonly DockerClient _dockerClient;

        private readonly ContainerService _containerService;

        public ContainerController(DockerClient dockerClient, ContainerService containerService)
        {
            _dockerClient = dockerClient;
            _containerService = containerService;
        }

        //public ContainerController()
        //{
        //    _dockerClient = new DockerClient();
        //}

        [HttpGet]
        [ProducesResponseType(typeof(IList<ContainerSummary>),200)]
        public async Task<ActionResult> GetContainers([FromQuery] ContainersListParameters containersListParameters, CancellationToken cancellationToken)
        {
            var response = await _containerService.GetContainers(containersListParameters, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: containers => Ok(containers)
                );
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateContainer([FromQuery] CreateContainerQueryParameters createContainerQueryParameters, [FromBody] ContainerCreateRequest createContainer, CancellationToken cancellationToken)
        {
            var response = await _containerService.CreateContainer(createContainerQueryParameters, createContainer, cancellationToken);    
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: containers => Ok(containers)
                );
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetContainer(string id, [FromQuery] ContainerInspectParameters containerInspectParameters, CancellationToken cancellationToken)
        {
            var response = await _containerService.GetContainer(id, containerInspectParameters, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error!.StatusCode, error.Message),
                Right: containers => Ok(containers)
            );
        }

        [HttpPost]
        [Route("{id}/restart")]
        public async Task<IActionResult> RestartContainer(string id, CancellationToken cancellationToken)
        {
            var response = await _containerService.RestartContainer(id, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(null)
                );
        }

        [HttpPost]
        [Route("{id}/start")]
        public async Task<IActionResult> StartContainer(string id, CancellationToken cancellationToken)
        {
            var response = await _containerService.StartContainer(id, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(null)
                );
        }

        [HttpPost]
        [Route("{id}/stop")]
        public async Task<IActionResult> StopContainer(string id, CancellationToken cancellationToken)
        {
            var response = await _containerService.StopContainer(id, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(null)
                );
        }

        [HttpPost]
        [Route("{id}/kill")]
        public async Task<IActionResult> KillContainer(string id, CancellationToken cancellationToken)
        {
            var response = await _containerService.KillContainer(id, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(null)
                );
        }

        [HttpPost]
        [Route("{id}/pause")]
        public async Task<IActionResult> PauseContainer(string id, CancellationToken cancellationToken)
        {
            var response = await _containerService.PauseContainer(id, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(null)
                );
        }

        [HttpPost]
        [Route("{id}/unpause")]
        public async Task<IActionResult> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            var response = await _containerService.UnpauseContainer(id, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(null)
                );
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteContainer(string id, [FromQuery]ContainerDeleteParameters parameters, CancellationToken cancellationToken)
        {
            var response = await _containerService.DeleteContainer(id, parameters, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(null)
                );
        }

        [HttpGet]
        [Route("{id}/logs")]
        public async System.Threading.Tasks.Task GetContainerLogs(string id, [FromQuery]ContainerLogsParameters parameters, CancellationToken cancellationToken)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Expected a WebSocket request.", cancellationToken: cancellationToken);
                return;
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var (success, stream, error) = await _containerService.GetContainerLogs(id, parameters, webSocket, cancellationToken);
        }

        [HttpGet]
        [Route("{id}/stats")]
        public async System.Threading.Tasks.Task GetContainerStats(string id, [FromQuery]ContainerStatsParameters parameters, CancellationToken cancellationToken)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await HttpContext.Response.WriteAsync("Expected a WebSocket request.");
                return;
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var (success, stream, error) = await _containerService.GetContainerStats(id, parameters, webSocket, cancellationToken);
        }

        [HttpPost]
        [Route("{id}/attach")]
        public async Task<Stream?> AttachToContainer(string id, ContainerAttachParameters containerAttachParameters, CancellationToken cancellationToken)
        {
            // TODO: Finish this properly and test it.
            try
            {
                HttpClient httpClient = _dockerClient.GetDockerHttpClient();
                string parameters = _dockerClient.GetQueryString(containerAttachParameters);
                HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, "", parameters);
                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                return stream;
            }
            catch (Exception ex)
            {
            }
            return Stream.Null;
        }

        [HttpPost]
        [Route("{id}/exec")]
        public async Task<IActionResult> CreateExec(string id, [FromBody] ContainerExecCreateParameters createParameters, CancellationToken cancellationToken)
        {
            var response = await _containerService.CreateExec(id, createParameters, cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(result)
                );
        }
    }
}
