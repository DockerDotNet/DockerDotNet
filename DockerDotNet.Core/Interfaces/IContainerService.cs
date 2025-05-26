using DockerDotNet.Shared.Models;

using LanguageExt;

using System.Net.WebSockets;

namespace DockerDotNet.Core.Interfaces
{
    public interface IContainerService
    {
        Task<Either<DockerError?, ContainerCreateResponse?>> CreateContainer(ContainerCreateParameters createContainerQueryParameters, ContainerCreateRequest createContainer, CancellationToken cancellationToken);
        //Task<Either<DockerError?, ContainerExecCreateResponse?>> CreateExec(string id, ContainerExecCreateParameters createParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> DeleteContainer(string id, ContainerDeleteParameters containerDeleteParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, ContainerInspectResponse?>> GetContainer(string id, ContainerInspectParameters queryParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, Stream?>> GetContainerLogs(string id, ContainerLogsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken);
        Task<Either<DockerError?, IList<ContainerSummary>?>> GetContainers(ContainersListParameters parameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, Stream?>> GetContainerStats(string id, ContainerStatsParameters parameters, WebSocket webSocket, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> KillContainer(string id, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> PauseContainer(string id, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> RestartContainer(string id, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> StartContainer(string id, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> StopContainer(string id, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> UnpauseContainer(string id, CancellationToken cancellationToken);
    }
}