using DockerDotNet.Core.Helpers;
using DockerDotNet.Core.Models;

using LanguageExt;

using Microsoft.Extensions.Logging;

using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;

namespace DockerDotNet.Core.Services
{
    public class ContainerService
    {
        private readonly DockerClient _dockerClient;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly StreamHelper streamHelper;
        private readonly ILogger<ContainerService> logger;

        public ContainerService(DockerClient dockerClient, JsonSerializerOptions jsonSerializerOptions, StreamHelper streamHelper, ILogger<ContainerService> logger)
        {
            _dockerClient = dockerClient;
            this.jsonSerializerOptions = jsonSerializerOptions;
            this.streamHelper = streamHelper;
            this.logger = logger;
        }

        public async Task<Either<DockerError?, IList<ContainerSummary>?>> GetContainers(ContainersListParameters parameters, CancellationToken cancellationToken)
        {
            string queryString = _dockerClient.GetQueryString(parameters);
            return await _dockerClient.GetAsync<IList<ContainerSummary>>("containers/json", queryString,  cancellationToken);
        }

        public async Task<Either<DockerError?, ContainerInspectResponse?>> GetContainer(string id, ContainerInspectParameters queryParameters, CancellationToken cancellationToken)
        {
            string parameters = _dockerClient.GetQueryString(queryParameters);
            return await _dockerClient.GetAsync<ContainerInspectResponse>($"containers/{id}/json", parameters,  cancellationToken);
        }

        public async Task<Either<DockerError?, ContainerCreateResponse?>> CreateContainer(CreateContainerQueryParameters createContainerQueryParameters, ContainerCreateRequest createContainer, CancellationToken cancellationToken)
        {
            string queryString = _dockerClient.GetQueryString(createContainerQueryParameters);
            return await _dockerClient.PostAsync<ContainerCreateResponse>("containers/create", queryString,  cancellationToken, body: JsonContent.Create(createContainer));
        }

        public async Task<Either<DockerError?, string?>> DeleteContainer(string id, ContainerDeleteParameters containerDeleteParameters, CancellationToken cancellationToken)
        {
            string queryString = _dockerClient.GetQueryString(containerDeleteParameters);
            return await _dockerClient.DeleteAsync<string>($"containers/{id}", queryString,  cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> RestartContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/restart", string.Empty,  cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> StartContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/start", string.Empty,  cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> StopContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/stop", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> KillContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/kill", string.Empty,  cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> PauseContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/pause", string.Empty,  cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<string>($"containers/{id}/unpause", string.Empty,  cancellationToken);
        }

        public async Task<Either<DockerError?, ContainerExecCreateResponse?>> CreateExec(string id, ContainerExecCreateParameters createParameters, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<ContainerExecCreateResponse>($"containers/{id}/exec", string.Empty,  cancellationToken, body: JsonContent.Create(createParameters));
        }

        public async Task<Either<DockerError?, Stream?>> GetContainerLogs(string id, ContainerLogsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                string query = _dockerClient.GetQueryString(parameters);

                return await  _dockerClient.GetStreamAsync($"containers/{id}/logs", query, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                return new DockerError(HttpStatusCode.RequestTimeout, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new DockerError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<Either<DockerError?, Stream?>> GetContainerStats(string id, ContainerStatsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                string query = _dockerClient.GetQueryString(parameters);

                return await _dockerClient.GetStreamAsync($"containers/{id}/stats", query, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                return new DockerError(HttpStatusCode.RequestTimeout, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new DockerError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

