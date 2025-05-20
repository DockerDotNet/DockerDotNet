using DockerDotNet.Core.Helpers;
using DockerDotNet.Core.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Services
{
    public class ImageService
    {
        private readonly DockerClient _dockerClient;
        private readonly StreamHelper streamHelper;

        public ImageService(DockerClient dockerClient, StreamHelper streamHelper)
        {
            _dockerClient = dockerClient;
            this.streamHelper = streamHelper;
        }

        public async Task<Either<DockerError?, IList<ImageSummary>?>> GetImages(ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(imagesListParameters);
            return await _dockerClient.GetAsync<IList<ImageSummary>>("images/json", query,  cancellationToken);
        }

        public async Task<Either<DockerError?, ImageInspect?>> GetImage(string name, CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<ImageInspect>($"images/{name}/json", string.Empty,  cancellationToken);
        }

        public async Task<Either<DockerError?, List<HistoryResponseItem>?>> GetImageHistory(string name, CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<List<HistoryResponseItem>>($"images/{name}/history", string.Empty,  cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> TagImage(string name, ImageTagParameters parameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(parameters);
            return await _dockerClient.PostAsync<string>($"images/{name}/tag", query,  cancellationToken);
        }

        public async Task<(bool, Stream?, DockerError?)> CreateImage(ImagesCreateParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
        {
            try
            {
                string query = _dockerClient.GetQueryString(parameters);

                AuthConfig authConfig = new AuthConfig
                {
                    Serveraddress = "",
                    Username = "",
                    Password = ""
                };
                Dictionary<string,string> authHeaders = _dockerClient.GetRegistryAuthHeaders(authConfig);

                var (success, pullStream, contentType, error) = await _dockerClient.PostStreamAsync($"images/create", query, cancellationToken, headers: authHeaders);

                if (success)
                {
                    var sendTask = System.Threading.Tasks.Task.Run(async () =>
                    {
                        await foreach (var stats in streamHelper.ReadNewlineDelimitedJson<CreateImageInfo>(pullStream, cancellationToken))
                        {
                            string line = JsonSerializer.Serialize(stats) + "\n";

                            if (string.IsNullOrWhiteSpace(line)) continue;
                            var bytes = Encoding.ASCII.GetBytes(line);

                            await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
                        }
                    }, cancellationToken);

                    await sendTask;

                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Session ended", CancellationToken.None);


                    return (true, pullStream, default);
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

        public async Task<Either<DockerError?, BuildPruneResponse?>> BuildPrune(BuildPruneParameters buildPruneParameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(buildPruneParameters);
            return await _dockerClient.PostAsync<BuildPruneResponse>("build/prune", query, cancellationToken);
        } 

        public async Task<Either<DockerError?, ImageDeleteResponseItem?>> DeleteImage(string name, ImageDeleteParameters parameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(parameters);
            return await _dockerClient.DeleteAsync<ImageDeleteResponseItem>($"images/{name}", query, cancellationToken);
        }
    }
}
