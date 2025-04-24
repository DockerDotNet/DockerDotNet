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
using System.Net.WebSockets;
using LanguageExt;

namespace DockerDotNet.Core.Services
{
    public class ContainerService
    {
        private readonly DockerClient _dockerClient;

        public ContainerService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
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

        public async Task<(bool, Stream?, DockerError?)> GetContainerLogs(string id, ContainerLogsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                string query = _dockerClient.GetQueryString(parameters);
                var (success, logStream, contentType, error) = await _dockerClient.GetStreamAsync($"containers/{id}/logs", query, cancellationToken);

                if (success && contentType == "application/vnd.docker.multiplexed-stream")
                {
                    var dockerStreamReader = new StreamReader(logStream!);

                    // Task to forward Docker output to WebSocket
                    var sendTask = System.Threading.Tasks.Task.Run(async () =>
                    {
                        var buffer = new byte[16384];
                        while (!dockerStreamReader.EndOfStream && webSocket.State == WebSocketState.Open)
                        {
                            string readLine = await dockerStreamReader.ReadLineAsync();
                            if (string.IsNullOrEmpty(readLine)) break;
                            var bytes = Encoding.ASCII.GetBytes(readLine);

                            await webSocket.SendAsync(bytes[8..], WebSocketMessageType.Text, true, cancellationToken);
                        }
                    });

                    await System.Threading.Tasks.Task.WhenAll(sendTask);

                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended", CancellationToken.None);

                    return (true, logStream, null);
                }
                else
                {
                    return (false, null, new DockerError(HttpStatusCode.InternalServerError, "Need to implement"));
                }

                
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, null, new DockerError(HttpStatusCode.RequestTimeout, ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, null, new DockerError(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        public async Task<(bool, Stream?, DockerError?)> GetContainerStats(string id, ContainerStatsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                string query = _dockerClient.GetQueryString(parameters);

                var (success, logStream, contentType, error) = await _dockerClient.GetStreamAsync($"containers/{id}/stats", query, cancellationToken);

                if (success && contentType == "application/vnd.docker.multiplexed-stream")
                {
                    var dockerStreamReader = new StreamReader(logStream!);

                    // Task to forward Docker output to WebSocket
                    var sendTask = System.Threading.Tasks.Task.Run(async () =>
                    {
                        var buffer = new byte[16384];
                        while (!dockerStreamReader.EndOfStream && webSocket.State == WebSocketState.Open)
                        {
                            string readLine = await dockerStreamReader.ReadLineAsync();
                            if (string.IsNullOrEmpty(readLine)) break;
                            var bytes = Encoding.ASCII.GetBytes(readLine);

                            await webSocket.SendAsync(bytes[8..], WebSocketMessageType.Text, true, cancellationToken);
                        }
                    });

                    await System.Threading.Tasks.Task.WhenAll(sendTask);

                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended", CancellationToken.None);

                    return (true, logStream, null);
                }
                else
                {
                    return (false, null, new DockerError(HttpStatusCode.InternalServerError, "Need to implement"));
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, null, new DockerError(HttpStatusCode.RequestTimeout, ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return (false, null, new DockerError(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}

