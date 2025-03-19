using DockerDotNet.Core.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Services
{
    public class ImageService
    {
        private readonly DockerClient _dockerClient;

        public ImageService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
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
    }
}
