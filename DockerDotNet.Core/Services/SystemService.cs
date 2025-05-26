using DockerDotNet.Core.Interfaces;
using DockerDotNet.Shared.Models;

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
    public class SystemService : ISystemService
    {
        private readonly DockerClient _dockerClient;

        public SystemService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }

        public async Task<Either<DockerError?, SystemVersion?>> GetVersionAsync(CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<SystemVersion>("version", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, SystemInfo?>> GetInfoAsync(CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<SystemInfo>("info", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, SystemAuthResponse?>> AuthenticateRegistry(AuthConfig authConfig, CancellationToken cancellationToken)
        {
            string query = _dockerClient.GetQueryString(authConfig);
            return await _dockerClient.PostAsync<SystemAuthResponse>("auth", query, cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> Ping_Get(CancellationToken cancellationToken)
        {
            return await _dockerClient.GetAsync<string>("_ping", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, SystemDataUsageResponse?>> GetDataUsageInformation(CancellationToken cancellationToken)
        {
            // TODO: Need to handle the enum configuration
            return await _dockerClient.GetAsync<SystemDataUsageResponse>("system/df", string.Empty, cancellationToken);
        }
    }
}
