using DockerDotNet.Core.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
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

        public ExecService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
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
                    var dockerToWebSocket = System.Threading.Tasks.Task.Run(async () =>
                    {
                        if (isTty)
                            await ReadRawStreamAsync(execStream, webSocket, cancellationToken);
                        else
                            await ReadMultiplexedStreamAsync(execStream, webSocket, cancellationToken);
                    }, cancellationToken);

                    var webSocketToDocker = System.Threading.Tasks.Task.Run(async () =>
                    {
                        var buffer = new byte[4096];
                        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                        {
                            var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                            if (result.MessageType == WebSocketMessageType.Close)
                                break;

                            // Handle input
                            var text = Encoding.UTF8.GetString(buffer, 0, result.Count).Replace("\r\n", "\n").Replace("\r", "\n");
                            if (!text.EndsWith("\n")) text += "\n";
                            var payload = Encoding.UTF8.GetBytes(text);

                            if (isTty)
                            {
                                await execStream.WriteAsync(payload.AsMemory(0, payload.Length), cancellationToken);
                            }
                            else
                            {
                                // Docker expects raw input to be prefixed with header: stream ID 0
                                var streamHeader = new byte[8];
                                streamHeader[0] = 0; // stdin
                                var lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(result.Count));
                                Array.Copy(lengthBytes, 0, streamHeader, 4, 4);

                                // somehow, not passing the header makes multiplexed stream work
                                //await execStream.WriteAsync(streamHeader, cancellationToken);
                                await execStream.WriteAsync(payload.AsMemory(0, payload.Length), cancellationToken);
                            }
                            await execStream.FlushAsync(cancellationToken);
                        }
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

        public async System.Threading.Tasks.Task ReadMultiplexedStreamAsync(Stream dockerStream, WebSocket clientWebSocket, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && clientWebSocket.State == WebSocketState.Open)
            {
                var header = new byte[8];
                int read = await dockerStream.ReadAsync(header.AsMemory(0, 8), ct);
                if (read == 0) break; // End of stream

                int payloadLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 4));
                int streamType = header[0]; // 0: stdin, 1: stdout, 2: stderr

                var payload = new byte[payloadLength];
                read = 0;
                while (read < payloadLength)
                {
                    int r = await dockerStream.ReadAsync(payload.AsMemory(read, payloadLength - read), ct);
                    if (r == 0) break;
                    read += r;
                }

                await clientWebSocket.SendAsync(payload, WebSocketMessageType.Text, true, ct);
            }

            if (clientWebSocket.State == WebSocketState.Open)
            {
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stream complete", ct);
            }
        }

        public async System.Threading.Tasks.Task ReadRawStreamAsync(
        Stream dockerStream,
        WebSocket ws,
        CancellationToken ct)
        {
            var buf = new byte[8192];
            while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
            {
                int r = await dockerStream.ReadAsync(buf, ct);
                if (r == 0) break;
                await ws.SendAsync(buf.AsMemory(0, r), WebSocketMessageType.Binary, true, ct);
            }
            if (ws.State == WebSocketState.Open)
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "EOF", ct);
        }
    }
}
