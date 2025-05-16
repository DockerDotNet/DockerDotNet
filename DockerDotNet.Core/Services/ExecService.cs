using DockerDotNet.Core.Helpers;
using DockerDotNet.Core.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Either<DockerError?, ContainerExecCreateResponse?>> CreateExec(string id, ExecConfig createExecParameters, CancellationToken cancellationToken)
        {
            return await _dockerClient.PostAsync<ContainerExecCreateResponse>($"containers/{id}/exec", string.Empty, cancellationToken, body: JsonContent.Create(createExecParameters));
        }


        public async Task<(bool, Stream?, DockerError?)> StartExecInstance(string id, ExecStartConfig execStartConfig, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                bool isTty = execStartConfig.Tty.GetValueOrDefault();
                //string query = _dockerClient.GetQueryString(execStartConfig);
                var (success, execStream, contentType, error) = await _dockerClient.PostStreamAsync($"exec/{id}/start", string.Empty, cancellationToken, body: JsonContent.Create(execStartConfig));
                if (success)
                {
                    // read docker output
                    var dockerToWebSocket = System.Threading.Tasks.Task.Run(async () =>
                    {
                        if (isTty)
                            await streamHelper.ReadRawStreamAsync(execStream, webSocket, cancellationToken);
                        else
                            await streamHelper.ReadMultiplexedStreamAsync(execStream, webSocket, cancellationToken);
                    }, cancellationToken);

                    // give docker input
                    var webSocketToDocker = System.Threading.Tasks.Task.Run(async () =>
                    {
                        if(isTty)
                            await streamHelper.WriteToRawStreamAsync(execStream, webSocket, cancellationToken);
                        else
                            await streamHelper.WriteToMultiplexedStreamAsync(execStream, webSocket, cancellationToken);
                    }, cancellationToken);

                    await System.Threading.Tasks.Task.WhenAny(dockerToWebSocket, webSocketToDocker);

                    return (true, execStream, null);
                }
                else
                {
                    return (false, null, error);
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
