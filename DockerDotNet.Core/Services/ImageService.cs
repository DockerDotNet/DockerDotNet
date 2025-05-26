using DockerDotNet.Core.Helpers;
using DockerDotNet.Core.Interfaces;
using DockerDotNet.Shared.Models;

using LanguageExt;

using Microsoft.Extensions.Logging;

using System.Net;
using System.Net.WebSockets;

namespace DockerDotNet.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly DockerClient _dockerClient;
        private readonly StreamHelper streamHelper;
        private readonly ILogger<ImageService> logger;

        public ImageService(DockerClient dockerClient, StreamHelper streamHelper, ILogger<ImageService> logger)
        {
            _dockerClient = dockerClient;
            this.streamHelper = streamHelper;
            this.logger = logger;
        }

        public async Task<Either<DockerError?, IList<ImageSummary>?>> GetImages(ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(imagesListParameters);
            return await _dockerClient.GetAsync<IList<ImageSummary>>("images/json", query, cancellationToken);
        }

        public async Task<Either<DockerError?, ImageInspect?>> GetImage(string name, CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<ImageInspect>($"images/{name}/json", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, List<HistoryResponseItem>?>> GetImageHistory(string name, CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<List<HistoryResponseItem>>($"images/{name}/history", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> TagImage(string name, ImageTagParameters parameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(parameters);
            return await _dockerClient.PostAsync<string>($"images/{name}/tag", query, cancellationToken);
        }

        public async Task<Either<DockerError?, Stream?>> CreateImage(ImagesCreateParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
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
                Dictionary<string, string> authHeaders = _dockerClient.GetRegistryAuthHeaders(authConfig);

                return await _dockerClient.PostStreamAsync($"images/create", query, cancellationToken, headers: authHeaders);
            }
            catch (OperationCanceledException ex)
            {
                return new DockerError(HttpStatusCode.RequestTimeout, ex.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);

                return new DockerError(HttpStatusCode.InternalServerError, ex.Message);
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

        public async Task<Either<DockerError?, List<ImageSearchResponseItem>?>> ImageSearch(ImageSearchParameters parameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(parameters);
            return await _dockerClient.GetAsync<List<ImageSearchResponseItem>>("images/search", query, cancellationToken).ConfigureAwait(false);
        }
    }
}
