using DockerDotNet.Core.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Services
{
    public class NetworkService
    {
        private readonly DockerClient _client;

        public NetworkService(DockerClient dockerClient)
        {
            this._client = dockerClient;
        }

        public async Task<Either<DockerError?, List<Network>?>> ListNetworks(NetworkListParameters networkListParameters, CancellationToken cancellationToken)
        {
            string query = _client.GetQueryString(networkListParameters);
            return await _client.GetAsync<List<Network>>("networks", query, cancellationToken);
        }
    }
}
