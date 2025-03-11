using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using DockerDotNet.Core;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/containers")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        ContainerService ContainerService { get; set; }

        public ContainerController(ContainerService containerService)
        {
            DockerClient = new DockerClient();
            ContainerService = containerService;
        }

        public ContainerController()
        {
            DockerClient = new DockerClient();
        }

        [HttpGet]
        public async Task<IList<ContainerListResponse>> GetContainers([FromQuery] ContainersListParameters containersListParameters, CancellationToken cancellationToken)
        {
            return await ContainerService.GetContainers(containersListParameters, cancellationToken);
        }

        [HttpPost]
        [Route("create")]
        public async Task<CreateContainerResponse> CreateContainer([FromQuery] CreateContainerQueryParameters createContainerQueryParameters, [FromBody] CreateContainerParameters createContainer, CancellationToken cancellationToken)
        {
            return await ContainerService.CreateContainer(createContainerQueryParameters, createContainer, cancellationToken);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ContainerInspectResponse> GetContainer(string id, [FromQuery] ContainerInspectParameters containerInspectParameters, CancellationToken cancellationToken)
        {
            return await ContainerService.GetContainer(id, containerInspectParameters, cancellationToken);
        }

        [HttpPost]
        [Route("{id}/restart")]
        public async Task<IActionResult> RestartContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await ContainerService.RestartContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/start")]
        public async Task<IActionResult> StartContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await ContainerService.StartContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/stop")]
        public async Task<IActionResult> StopContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await ContainerService.StopContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/kill")]
        public async Task<IActionResult> KillContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await ContainerService.KillContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/pause")]
        public async Task<IActionResult> PauseContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await ContainerService.PauseContainer(id, cancellationToken));
        }

        [HttpPost]
        [Route("{id}/unpause")]
        public async Task<IActionResult> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            return Ok(await ContainerService.UnpauseContainer(id, cancellationToken));
        }


        [HttpGet]
        [Route("{id}/logs")]
        public async Task GetContainerLogs(string id, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient httpClient = DockerClient.GetDockerHttpClient();

                HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, $"containers/{id}/logs?follow=true&stdout=true&tail=50", string.Empty);

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

        [HttpGet]
        [Route("{id}/stats")]
        public async Task GetContainerStats(string id, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient httpClient = DockerClient.GetDockerHttpClient();

                HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, $"containers/{id}/stats?stream=true", string.Empty);

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

        [HttpPost]
        [Route("{id}/attach")]
        public async Task<Stream?> AttachToContainer(string id, ContainerAttachParameters containerAttachParameters, CancellationToken cancellationToken)
        {
            // TODO: Finish this properly and test it.
            try
            {
                HttpClient httpClient = DockerClient.GetDockerHttpClient();
                string parameters = DockerClient.GetQueryString(containerAttachParameters);
                HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, "", parameters);
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
            return await ContainerService.CreateExec(id, createParameters, cancellationToken);
        }
    }
}
