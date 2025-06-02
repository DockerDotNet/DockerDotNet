using DockerDotNet.Shared.Helpers;
using DockerDotNet.Shared.Interfaces;
using DockerDotNet.Shared.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.Relay.Services
{
    public class ContainerRelayService : IContainerService
    {
        private readonly HttpClientHelper _clientHelper;
        private Uri? baseAddress = new UriBuilder("https://localhost:7075/api/").Uri;

        public ContainerRelayService(HttpClientHelper clientHelper)
        {
            this._clientHelper = clientHelper;
        }

        public Task<Either<DockerError?, ContainerCreateResponse?>> CreateContainer(ContainerCreateParameters createContainerQueryParameters, ContainerCreateRequest createContainer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, string?>> DeleteContainer(string id, ContainerDeleteParameters containerDeleteParameters, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, ContainerInspectResponse?>> GetContainer(string id, ContainerInspectParameters queryParameters, CancellationToken cancellationToken)
        {
            string query = _clientHelper.GetQueryString(queryParameters);
            HttpRequestMessage requestMessage = _clientHelper.PrepareHttpRequestMessage(baseAddress, HttpMethod.Get, $"containers/{id}", query);

            return _clientHelper.SendRequestAsync<ContainerInspectResponse>(requestMessage, cancellationToken); 
            //throw new NotImplementedException();
        }

        public Task<Either<DockerError?, Stream?>> GetContainerLogs(string id, ContainerLogsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, IList<ContainerSummary>?>> GetContainers(ContainersListParameters parameters, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, Stream?>> GetContainerStats(string id, ContainerStatsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, string?>> KillContainer(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, string?>> PauseContainer(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, string?>> RestartContainer(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, string?>> StartContainer(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, string?>> StopContainer(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Either<DockerError?, string?>> UnpauseContainer(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
