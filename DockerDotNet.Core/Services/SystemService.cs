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

        public async Task<(bool, Core.Models.SystemVersion?, DockerError?)> GetVersionAsync(CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<Core.Models.SystemVersion>("version", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, Core.Models.SystemInfo?, DockerError?)> GetInfoAsync(CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<Core.Models.SystemInfo>("info", string.Empty, _serializerOptions, cancellationToken);
        }

        public async Task<(bool, SystemAuthResponse?, DockerError?)> AuthenticateRegistry(AuthConfig authConfig, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(authConfig);
            return await _dockerClient.PostAsync<SystemAuthResponse>("auth", query, _serializerOptions, cancellationToken);
        }
    }
}
