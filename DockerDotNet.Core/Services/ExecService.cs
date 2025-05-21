using DockerDotNet.Core.Helpers;
using DockerDotNet.Core.Models;

using LanguageExt;

using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;

namespace DockerDotNet.Core.Services
{
    public class ExecService
    {
        private readonly DockerClient _dockerClient;
        private readonly StreamHelper streamHelper;

        public ExecService(DockerClient dockerClient, StreamHelper streamHelper)
        {
            _dockerClient = dockerClient;
            this.streamHelper = streamHelper;
        }

        public async Task<Either<DockerError?, ExecInspectResponse?>> InspectExec(string id, CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<ExecInspectResponse>($"exec/{id}/json", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, ContainerExecCreateResponse?>> CreateExec(string id, ExecConfig createExecParameters, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<ContainerExecCreateResponse>($"containers/{id}/exec", string.Empty, cancellationToken, body: JsonContent.Create(createExecParameters));
        }

        public async Task<Either<DockerError?, Stream?>> StartExecInstance(string id, ExecStartConfig execStartConfig, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                bool isTty = execStartConfig.Tty.GetValueOrDefault();
                //string query = _dockerClient.GetQueryString(execStartConfig);
                return await _dockerClient.PostHijackedStreamAsync($"exec/{id}/start", string.Empty, cancellationToken, body: JsonContent.Create(execStartConfig));
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.ToString());
                return new DockerError(HttpStatusCode.RequestTimeout, ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new DockerError(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<Either<DockerError?, string?>> ReizeExec(string id, int height, int width, CancellationToken cancellationToken)
        {
            string query = $"h={height}&w={width}";
            return await _dockerClient.PostAsync<string?>($"exec/{id}/resize", query, cancellationToken);
        }
    }
}
