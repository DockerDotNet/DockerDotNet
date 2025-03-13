using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using DockerDotNet.Core;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/containers")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        private readonly DockerClient _dockerClient;

        private readonly ContainerService _containerService;

        public ContainerController(ContainerService containerService)
        {
            _dockerClient = new DockerClient();
            _containerService = containerService;
        }

        //public ContainerController()
        //{
        //    _dockerClient = new DockerClient();
        //}

        [HttpGet]
        public async Task<IList<ContainerListResponse>> GetContainers([FromQuery] ContainersListParameters containersListParameters, CancellationToken cancellationToken)
        {
            return await _containerService.GetContainers(containersListParameters, cancellationToken);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateContainer([FromQuery] CreateContainerQueryParameters createContainerQueryParameters, [FromBody] ContainerCreateRequest createContainer, CancellationToken cancellationToken)
        {
            return Ok(await _containerService.CreateContainer(createContainerQueryParameters, createContainer, cancellationToken));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetContainer(string id, [FromQuery] ContainerInspectParameters containerInspectParameters, CancellationToken cancellationToken)
        {
            var(success, response, error) = await _containerService.GetContainer(id, containerInspectParameters, cancellationToken);
            return success? Ok(response) : StatusCode((int)error!.StatusCode, error.Message);
            //return await _containerService.GetContainer(id, containerInspectParameters, cancellationToken);
        }

        [HttpPost]
        [Route("{id}/restart")]
        public async Task<IActionResult> RestartContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await _containerService.RestartContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/start")]
        public async Task<IActionResult> StartContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await _containerService.StartContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/stop")]
        public async Task<IActionResult> StopContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await _containerService.StopContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/kill")]
        public async Task<IActionResult> KillContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await _containerService.KillContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/pause")]
        public async Task<IActionResult> PauseContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await _containerService.PauseContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/unpause")]
        public async Task<IActionResult> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await _containerService.UnpauseContainer(id, cancellationToken));
        }

        [HttpGet]
        [Route("{id}/logs")]
        public async System.Threading.Tasks.Task GetContainerLogs(string id, CancellationToken cancellationToken)
        {
            var (logStream, statusCode, contentType) = await _containerService.GetContainerLogs(id, cancellationToken);

            Response.StatusCode = (int)statusCode;
            Response.ContentType = contentType;

            if (logStream != null)
            {
                await logStream.CopyToAsync(Response.Body, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }

        [HttpGet]
        [Route("{id}/stats")]
        public async System.Threading.Tasks.Task GetContainerStats(string id, CancellationToken cancellationToken)
        {
            var (logStream, statusCode, contentType) = await _containerService.GetContainerStats(id, cancellationToken);

            Response.StatusCode = (int)statusCode;
            Response.ContentType = contentType;

            if (logStream != null)
            {
                await logStream.CopyToAsync(Response.Body, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
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
        public async Task<ContainerExecCreateResponse> CreateExec(string id, [FromBody] ContainerExecCreateParameters createParameters, CancellationToken cancellationToken)
        {
            return await _containerService.CreateExec(id, createParameters, cancellationToken);
        }
    }
}
