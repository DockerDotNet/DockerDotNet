using DockerDotNet.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

namespace DockerDotNet.Core.Services
{
    public class ContainerService
    {
        private readonly DockerClient _dockerClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public ContainerService(DockerClient dockerClient, JsonSerializerOptions jsonSerializerOptions)
        {
            _dockerClient = dockerClient;
            _serializerOptions = jsonSerializerOptions;
        }

        public async Task<(bool, IList<ContainerSummary>?, DockerError?)> GetContainers(ContainersListParameters parameters, CancellationToken cancellationToken)
        {
            string queryString = _dockerClient.GetQueryString(parameters);
            return await _dockerClient.GetRequestAsync<IList<ContainerSummary>>("containers/json", queryString, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, ContainerInspectResponse?, DockerError?)> GetContainer(string id, ContainerInspectParameters queryParameters, CancellationToken cancellationToken)
        {
            string parameters = _dockerClient.GetQueryString(queryParameters);
            return await _dockerClient.GetRequestAsync<ContainerInspectResponse>($"containers/{id}/json", parameters, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, ContainerCreateResponse?, DockerError?)> CreateContainer(CreateContainerQueryParameters createContainerQueryParameters, ContainerCreateRequest createContainer, CancellationToken cancellationToken)
        {
            string queryString = _dockerClient.GetQueryString(createContainerQueryParameters);

            return await _dockerClient.PostAsync<ContainerCreateResponse>("containers/create", queryString, _serializerOptions, cancellationToken, JsonContent.Create(createContainer));

            //using HttpClient httpClient = _dockerClient.GetDockerHttpClient();
            //HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, "containers/create", queryString, JsonContent.Create(createContainer));

            //httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            //HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);

            ////httpResponseMessage.EnsureSuccessStatusCode();

            //ContainerCreateResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerCreateResponse>(cancellationToken);
        }

        public async Task<string> RestartContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/restart", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> StartContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/start", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> StopContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/stop", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> KillContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/kill", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> PauseContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/pause", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<string> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/unpause", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            return responseContent;
        }

        public async Task<ContainerExecCreateResponse> CreateExec(string id, ContainerExecCreateParameters createParameters, CancellationToken cancellationToken)
        {
            HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Post, $"containers/{id}/exec", string.Empty, JsonContent.Create(createParameters));
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            ContainerExecCreateResponse? response = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerExecCreateResponse>(cancellationToken);
            return response;
        }

        public async Task<(Stream?, HttpStatusCode, string)> GetContainerLogs(string id, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient httpClient = _dockerClient.GetDockerHttpClient();

                HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Get, $"containers/{id}/logs?follow=true&stdout=true&tail=50", string.Empty);

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                string contentType = httpResponseMessage.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                return  (await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken), httpResponseMessage.StatusCode, contentType);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, HttpStatusCode.RequestTimeout, "application/octet-stream");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, HttpStatusCode.InternalServerError, "application/octet-stream");
            }
        }

        public async Task<(Stream?, HttpStatusCode, string)> GetContainerStats(string id, CancellationToken cancellationToken)
        {
            try
            {
                HttpClient httpClient = _dockerClient.GetDockerHttpClient();

                HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Get, $"containers/{id}/stats?stream=true", string.Empty);

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                string contentType = httpResponseMessage.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

                return (await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken), httpResponseMessage.StatusCode, contentType);
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, HttpStatusCode.RequestTimeout, "application/octet-stream");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (null, HttpStatusCode.InternalServerError, "application/octet-stream");
            }
        }
    }
}

