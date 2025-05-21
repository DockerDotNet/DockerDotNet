using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace DockerDotNet.Core.Helpers
{
    public class StreamHelper
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public StreamHelper(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task ReadMultiplexedStreamAsync(Stream stream, WebSocket webSocket, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && webSocket.State == WebSocketState.Open)
            {
                var header = new byte[8];
                int read = await stream.ReadAsync(header.AsMemory(0, 8), ct);
                if (read == 0) break; // End of stream

                int payloadLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 4));
                int streamType = header[0]; // 0: stdin, 1: stdout, 2: stderr

                var payload = new byte[payloadLength];
                read = 0;
                while (read < payloadLength)
                {
                    int r = await stream.ReadAsync(payload.AsMemory(read, payloadLength - read), ct);
                    if (r == 0) break;
                    read += r;
                }

                await webSocket.SendAsync(payload, WebSocketMessageType.Text, true, ct);
            }

            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Stream complete", ct);
            }
        }

        public async Task ReadRawStreamAsync(Stream stream, WebSocket ws, CancellationToken ct)
        {
            var buf = new byte[8192];
            while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
            {
                int r = await stream.ReadAsync(buf, ct);
                if (r == 0) break;
                await ws.SendAsync(buf.AsMemory(0, r), WebSocketMessageType.Binary, true, ct);
            }
            if (ws.State == WebSocketState.Open)
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "EOF", ct);
        }

        public async Task WriteToRawStreamAsync(Stream stream, WebSocket webSocket, CancellationToken cancellationToken)
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var buffer = new byte[4096];

                var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                    return;

                // Handle input
                var text = Encoding.UTF8.GetString(buffer, 0, result.Count).Replace("\r\n", "\n").Replace("\r", "\n");
                if (!text.EndsWith("\n")) text += "\n";
                var payload = Encoding.UTF8.GetBytes(text);

                await stream.WriteAsync(payload.AsMemory(0, payload.Length), cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
        }

        public async Task WriteToMultiplexedStreamAsync(Stream stream, WebSocket webSocket, CancellationToken cancellationToken)
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var buffer = new byte[4096];
                var result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                    return;

                // Handle input
                var text = Encoding.UTF8.GetString(buffer, 0, result.Count).Replace("\r\n", "\n").Replace("\r", "\n");
                if (!text.EndsWith("\n")) text += "\n";
                var payload = Encoding.UTF8.GetBytes(text);


                // Docker expects raw input to be prefixed with header: stream ID 0
                var streamHeader = new byte[8];
                streamHeader[0] = 0; // stdin
                var lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(result.Count));
                Array.Copy(lengthBytes, 0, streamHeader, 4, 4);

                // somehow, not passing the header makes multiplexed stream work
                //await execStream.WriteAsync(streamHeader, cancellationToken);
                await stream.WriteAsync(payload.AsMemory(0, payload.Length), cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Convert newlineDelimitedStream into an enumerable of Json type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<T> ReadNewlineDelimitedJson<T>(Stream stream, [EnumeratorCancellation] CancellationToken ct = default)
        {
            using var reader = new StreamReader(stream, leaveOpen: false);

            string? line;
            while ((line = await reader.ReadLineAsync(ct)) is not null)
            {
                if (line.Length is 0) continue; // keep‑alive ping

                T? stats =
                    JsonSerializer.Deserialize<T>(line, jsonSerializerOptions);

                if (stats is not null)
                    yield return stats;
            }
        }

        public async Task HandleNDJsonStreamAsync<T>(Stream stream, WebSocket webSocket, CancellationToken cancellationToken)
        {
            var sendTask = System.Threading.Tasks.Task.Run(async () =>
            {
                await foreach (var stats in ReadNewlineDelimitedJson<T>(stream, cancellationToken))
                {
                    string line = JsonSerializer.Serialize(stats) + "\n";

                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var bytes = Encoding.ASCII.GetBytes(line);

                    await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
                }
            }, cancellationToken);

            await sendTask;

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended", CancellationToken.None);
        }

        public async Task HandleMultiplexedStreamAsync(bool isTty,Stream stream, WebSocket WebSocket, CancellationToken cancellationToken)
        {
            // read docker output
            var dockerToWebSocket = System.Threading.Tasks.Task.Run(async () =>
            {
                if (isTty)
                    await ReadRawStreamAsync(stream, WebSocket, cancellationToken);
                else
                    await ReadMultiplexedStreamAsync(stream, WebSocket, cancellationToken);
            }, cancellationToken);

            // give docker input
            var webSocketToDocker = System.Threading.Tasks.Task.Run(async () =>
            {
                if (isTty)
                    await WriteToRawStreamAsync(stream, WebSocket, cancellationToken);
                else
                    await WriteToMultiplexedStreamAsync(stream, WebSocket, cancellationToken);
            }, cancellationToken);

            await System.Threading.Tasks.Task.WhenAny(dockerToWebSocket, webSocketToDocker);
        }
    }
}
