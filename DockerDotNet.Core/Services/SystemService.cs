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
    public class SystemService
    {
        private readonly DockerClient _dockerClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public SystemService(DockerClient dockerClient, JsonSerializerOptions serializerOptions)
        {
            _dockerClient = dockerClient;
            _serializerOptions = serializerOptions;
        }

        public async Task<Core.Models.SystemVersion> GetVersionAsync()
        {
            HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Get, "version", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            return await httpResponseMessage.Content.ReadFromJsonAsync<Core.Models.SystemVersion>(_serializerOptions);
        }

        public async Task<Core.Models.SystemInfo> GetInfoAsync()
        {
            HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Get, "info", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            return await httpResponseMessage.Content.ReadFromJsonAsync<Core.Models.SystemInfo>(_serializerOptions);
        }
    }
}
