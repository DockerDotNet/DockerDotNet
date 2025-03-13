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

        public SystemService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }

        public async Task<Core.Models.Version> GetVersionAsync()
        {
            HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Get, "version", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new PlatformJsonConverter());
            jsonSerializerOptions.Converters.Add(new ComponentJsonConverter());
            jsonSerializerOptions.Converters.Add(new VersionJsonConverter());

            return await httpResponseMessage.Content.ReadFromJsonAsync<Core.Models.Version>(jsonSerializerOptions);
        }

        public async Task<Core.Models.Info> GetInfoAsync()
        {
            HttpClient httpClient = _dockerClient.GetDockerHttpClient();

            HttpRequestMessage requestMessage = _dockerClient.PrepareHttpRequest(HttpMethod.Get, "", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage);

            return await httpResponseMessage.Content.ReadFromJsonAsync<Core.Models.Info>();
        }
    }
}
