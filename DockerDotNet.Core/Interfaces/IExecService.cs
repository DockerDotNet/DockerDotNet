using DockerDotNet.Core.Models;

using LanguageExt;

using System.Net.WebSockets;

namespace DockerDotNet.Core.Interfaces
{
    public interface IExecService
    {
        Task<Either<DockerError?, ContainerExecCreateResponse?>> CreateExec(string id, ExecConfig createExecParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, ExecInspectResponse?>> InspectExec(string id, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> ReizeExec(string id, int height, int width, CancellationToken cancellationToken);
        Task<Either<DockerError?, Stream?>> StartExecInstance(string id, ExecStartConfig execStartConfig, WebSocket webSocket, CancellationToken cancellationToken);
    }
}