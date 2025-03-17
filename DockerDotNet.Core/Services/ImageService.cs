using DockerDotNet.Core.Models;

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
        private readonly JsonSerializerOptions _serializerOptions;

        public ImageService(DockerClient dockerClient, JsonSerializerOptions serializerOptions)
        {
            _dockerClient = dockerClient;
            _serializerOptions = serializerOptions;
        }

        public async Task<(bool, IList<ImageSummary>?, DockerError?)> GetImages(ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(imagesListParameters);
            return await _dockerClient.GetAsync<List<ImageSummary>>("images/json", query, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, ImageInspect?, DockerError?)> GetImage(string name, CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<ImageInspect>($"images/{name}/json", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, List<HistoryResponseItem>?, DockerError?)> GetImageHistory(string name, CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<List<HistoryResponseItem>>($"images/{name}/history", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, string?, DockerError?)> TagImage(string name, ImageTagParameters parameters, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(parameters);
            return await _dockerClient.PostAsync<string>($"images/{name}/tag", query, _serializerOptions, cancellationToken);
        }
    }
}
