using DockerDotNet.Core.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
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

        public async Task<Either<DockerError?, Network?>> InspectNetwork(string name, NetworkInspectParameters networkInspectParameters, CancellationToken cancellationToken)
        {
            string query = _client.GetQueryString(networkInspectParameters);
            return await _client.GetAsync<Network>($"networks/{name}", query, cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> RemoveNetwork(string name, CancellationToken cancellationToken)
        {
            return await _client.DeleteAsync<string>($"networks/{name}", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, NetworkCreateResponse?>> CreateNetwork(NetworkCreateRequest networkCreateRequest, CancellationToken cancellationToken)
        {
            return await _client.PostAsync<NetworkCreateResponse>("networks/create", string.Empty, cancellationToken, body: JsonContent.Create(networkCreateRequest));
        }

        public async Task<Either<DockerError?, string?>> ConnectContainer(string id, NetworkConnectRequest networkConnectRequest, CancellationToken cancellationToken)
        {
            return await _client.PostAsync<string>($"networks/{id}/connect", string.Empty, cancellationToken, body: JsonContent.Create(networkConnectRequest));
        }

        public async Task<Either<DockerError?, string?>> DisconnectContainer(string id, NetworkDisconnectRequest networkDisconnectRequest, CancellationToken cancellationToken)
        {
            return await _client.PostAsync<string>($"networks/{id}/disconnect", string.Empty, cancellationToken, body: JsonContent.Create(networkDisconnectRequest));
        }

        public async Task<Either<DockerError?, NetworkPruneResponse?>> PruneNetwork(NetworkPruneParameters networkPruneParameters, CancellationToken cancellationToken)
        {
            string query = _client.GetQueryString(networkPruneParameters);
            return await _client.PostAsync<NetworkPruneResponse>("networks/prune", query, cancellationToken);
        }
    }
}
