using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using DockerDotNet.Core;
using DockerDotNet.Core.Models;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/containers")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        public ContainerController()
        {
            DockerClient = new DockerClient();
        }

        [HttpGet]
        public async Task<IList<ContainerListResponse>> GetContainers([FromQuery] ContainersListParameters containersListParameters, CancellationToken cancellationToken)
        {
            string queryString = DockerClient.GetQueryString(containersListParameters);

            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, "containers/json", queryString);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();

            IList<ContainerListResponse>? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<IList<ContainerListResponse>>(cancellationToken);
            return responseContent;
        }

        [HttpPost]
        [Route("{create}")]
        public async Task<CreateContainerResponse> CreateContainer([FromQuery] CreateContainerQueryParameters createContainerQueryParameters, [FromBody] CreateContainerParameters createContainer, CancellationToken cancellationToken)
        {
            string queryString = DockerClient.GetQueryString(createContainerQueryParameters);

            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, "containers/create", queryString, JsonContent.Create(createContainer));

            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();

            CreateContainerResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<CreateContainerResponse>(cancellationToken);
            return responseContent;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ContainerInspectResponse> GetContainer(string id, [FromQuery] ContainerInspectParameters containerInspectParameters, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            string parameters = DockerClient.GetQueryString(containerInspectParameters);

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, $"containers/{id}/json", parameters);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            ContainerInspectResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerInspectResponse>(jsonSerializerOptions, cancellationToken);
            //string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        [HttpPost]
        [Route("{id}/restart")]
        public async Task<IActionResult> RestartContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/restart", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return Ok(responseContent);
        }

        [HttpPost]
        [Route("{id}/start")]
        public async Task<IActionResult> StartContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/start", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return Ok(responseContent);
        }

        [HttpPost]
        [Route("{id}/stop")]
        public async Task<IActionResult> StopContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/stop", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return Ok(responseContent);
        }

        [HttpPost]
        [Route("{id}/kill")]
        public async Task<IActionResult> KillContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/kill", string.Empty);
            
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return Ok(responseContent);
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
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/exec", string.Empty, JsonContent.Create(createParameters));
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            ContainerExecCreateResponse? response = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerExecCreateResponse>(cancellationToken);
            return response;
        }
    }
}
