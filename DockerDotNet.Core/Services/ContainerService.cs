using DockerDotNet.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Services
{
    public class ContainerService
    {
        private DockerClient DockerClient { get; set; }

        public ContainerService(DockerClient dockerClient)
        {
            DockerClient = dockerClient;
        }

        public async Task<IList<ContainerListResponse>> GetContainers(ContainersListParameters parameters, CancellationToken cancellationToken)
        {
            string queryString = DockerClient.GetQueryString(parameters);

            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, "containers/json", queryString);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();

            IList<ContainerListResponse>? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<IList<ContainerListResponse>>(cancellationToken);
            return responseContent;
        }

        public async Task<ContainerInspectResponse> GetContainer(string id, ContainerInspectParameters queryParameters, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            string parameters = DockerClient.GetQueryString(queryParameters);

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, $"containers/{id}/json", parameters);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            ContainerInspectResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerInspectResponse>(jsonSerializerOptions, cancellationToken);
            //string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;

        }

        public async Task<CreateContainerResponse> CreateContainer(CreateContainerQueryParameters createContainerQueryParameters, CreateContainerParameters createContainer, CancellationToken cancellationToken)
        {
            string queryString = DockerClient.GetQueryString(createContainerQueryParameters);

            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, "containers/create", queryString, JsonContent.Create(createContainer));

            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

            //httpResponseMessage.EnsureSuccessStatusCode();

            CreateContainerResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<CreateContainerResponse>(cancellationToken);
            return responseContent;
        }

        public async Task<string> RestartContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/restart", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> StartContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/start", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> StopContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/stop", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> KillContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/kill", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> PauseContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/pause", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/unpause", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<ContainerExecCreateResponse> CreateExec(string id, ContainerExecCreateParameters createParameters, CancellationToken cancellationToken)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/exec", string.Empty, JsonContent.Create(createParameters));
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            ContainerExecCreateResponse? response = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerExecCreateResponse>(cancellationToken);
            return response;
        }
    }
}
