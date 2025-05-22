using DockerDotNet.Core.Models;

using LanguageExt;

namespace DockerDotNet.Core.Interfaces
{
    public interface INetworkService
    {
        Task<Either<DockerError?, string?>> ConnectContainer(string id, NetworkConnectRequest networkConnectRequest, CancellationToken cancellationToken);
        Task<Either<DockerError?, NetworkCreateResponse?>> CreateNetwork(NetworkCreateRequest networkCreateRequest, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> DisconnectContainer(string id, NetworkDisconnectRequest networkDisconnectRequest, CancellationToken cancellationToken);
        Task<Either<DockerError?, Network?>> InspectNetwork(string name, NetworkInspectParameters networkInspectParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, List<Network>?>> ListNetworks(NetworkListParameters networkListParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, NetworkPruneResponse?>> PruneNetwork(NetworkPruneParameters networkPruneParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> RemoveNetwork(string name, CancellationToken cancellationToken);
    }
}