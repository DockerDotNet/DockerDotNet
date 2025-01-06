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
        public async Task<string> GetContainers()
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new UriBuilder($"{httpClient.BaseAddress}containers/json").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            httpClient.Dispose();

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<string> GetContainer(string id)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/json").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            httpClient.Dispose();

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        [HttpPost]
        [Route("{id}/restart")]
        public async Task<string> RestartContainer(string id)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/restart").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            httpClient.Dispose();

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        [HttpPost]
        [Route("{id}/start")]
        public async Task<string> StartContainer(string id)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/start").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            httpClient.Dispose();

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        [HttpPost]
        [Route("{id}/stop")]
        public async Task<string> StopContainer(string id)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/stop").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            httpClient.Dispose();

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        [HttpPost]
        [Route("{id}/kill")]
        public async Task<string> KillContainer(string id)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/kill").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            httpClient.Dispose();

            return await httpResponseMessage.Content.ReadAsStringAsync();
        }

        [HttpGet]
        [Route("{id}/logs")]
        public async Task GetContainerLogs(string id)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new UriBuilder($"{httpClient.BaseAddress}containers/{id}/logs?follow=true&stdout=true&tail=50").Uri);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);


            Response.StatusCode = (int)httpResponseMessage.StatusCode;
            Response.ContentType = httpResponseMessage.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

            using var upstreamStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            await upstreamStream.CopyToAsync(Response.Body);
            await Response.Body.FlushAsync();
        }
    }
}
