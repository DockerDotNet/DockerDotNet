using DockerDotNet.Shared;
using DockerDotNet.Shared.Helpers;
using DockerDotNet.Shared.Interfaces;
using DockerDotNet.Shared.Models;

using LanguageExt;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
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

        public async Task<Either<DockerError?, Stream?>> GetContainerLogs(string id, ContainerLogsParameters parameters, CancellationToken cancellationToken)
        {
            Uri webSockerUri = new Uri("wss://localhost:7075/api/containers/67a9de123f68f8e7db1965813198aa2da7c0b36c96f9d94058e2295efe75bb47/logs?follow=false&stdout=true&tail=all");
            SocketsHttpHandler handler = new SocketsHttpHandler();
            ClientWebSocket ws = new();

            //ws.Options.HttpVersion = HttpVersion.Version20;
            //ws.Options.HttpVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
            
            await ws.ConnectAsync(webSockerUri, new HttpMessageInvoker(handler), cancellationToken);

            if (ws.State == WebSocketState.Open)
                return new WebSocketStream(ws);

            return new DockerError(HttpStatusCode.BadRequest, "Stream failed");
            
            var memoryStream = new MemoryStream();

            while (ws.State == WebSocketState.Open)
            {
                byte[] bytes = new byte[4096];
                await ws.ReceiveAsync(bytes, cancellationToken);
                await memoryStream.WriteAsync(bytes, 0, bytes.Length);
            }

            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);

        }

        public Task<Either<DockerError?, IList<ContainerSummary>?>> GetContainers(ContainersListParameters parameters, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Either<DockerError?, Stream?>> GetContainerStats(string id, ContainerStatsParameters parameters, CancellationToken cancellationToken)
        {
            Uri webSockerUri = new Uri("wss://localhost:7075/api/containers/67a9de123f68f8e7db1965813198aa2da7c0b36c96f9d94058e2295efe75bb47/stats?stream=true");
            SocketsHttpHandler handler = new SocketsHttpHandler();
            ClientWebSocket ws = new();

            //ws.Options.HttpVersion = HttpVersion.Version20;
            //ws.Options.HttpVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;

            await ws.ConnectAsync(webSockerUri, new HttpMessageInvoker(handler), cancellationToken);

            if (ws.State == WebSocketState.Open)
                return new WebSocketStream(ws);

            return new DockerError(HttpStatusCode.BadRequest, "Stream failed");
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
