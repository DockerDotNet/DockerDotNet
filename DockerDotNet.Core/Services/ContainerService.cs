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
        }

        public async Task<(bool, string?, DockerError?)> RestartContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/restart", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, string?, DockerError?)> StartContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/start", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, string?, DockerError?)> StopContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/stop", string.Empty, _serializerOptions,cancellationToken);
        }

        public async Task<(bool, string?, DockerError?)> KillContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/kill", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, string?, DockerError?)> PauseContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/pause", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, string?, DockerError?)> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/unpause", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, ContainerExecCreateResponse?, DockerError?)> CreateExec(string id, ContainerExecCreateParameters createParameters, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<ContainerExecCreateResponse>($"containers/{id}/exec", string.Empty, _serializerOptions, cancellationToken, JsonContent.Create(createParameters));
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

