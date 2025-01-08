using DockerDotNet.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetContainers(CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            Uri requestUri = new UriBuilder($"{httpClient.BaseAddress}containers/json").Uri;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return Ok(responseContent);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            Uri requestUri = new UriBuilder($"{httpClient.BaseAddress}containers/{id}/json").Uri;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return Ok(responseContent);
        }

        [HttpPost]
        [Route("{id}/restart")]
        public async Task<IActionResult> RestartContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            Uri requestUri = new UriBuilder($"{httpClient.BaseAddress}containers/{id}/restart").Uri;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
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
            Uri requestUri = new UriBuilder($"{httpClient.BaseAddress}containers/{id}/start").Uri;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
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
            Uri requestUri = new UriBuilder($"{httpClient.BaseAddress}containers/{id}/stop").Uri;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
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
            Uri requestUri = new UriBuilder($"{httpClient.BaseAddress}containers/{id}/kill").Uri;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
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

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/logs?follow=true&stdout=true&tail=50").Uri);
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

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/stats?stream=true").Uri);
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
