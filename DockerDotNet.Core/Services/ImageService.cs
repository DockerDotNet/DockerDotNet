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

        public async Task<IList<ImageSummary>> GetImages(ImagesListParameters imagesListParameters, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = _dockerClient.GetDockerHttpClient();
            string parameters = _dockerClient.GetQueryString(imagesListParameters);
            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Get, "images/json", parameters, null);
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            return await httpResponseMessage.Content.ReadFromJsonAsync<IList<ImageSummary>>(_serializerOptions ,cancellationToken);
        }

    }
}
