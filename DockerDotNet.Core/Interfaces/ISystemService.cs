using DockerDotNet.Shared.Models;

using LanguageExt;

namespace DockerDotNet.Core.Interfaces
{
    public interface ISystemService
    {
        Task<Either<DockerError?, SystemAuthResponse?>> AuthenticateRegistry(AuthConfig authConfig, CancellationToken cancellationToken);
        Task<Either<DockerError?, SystemDataUsageResponse?>> GetDataUsageInformation(CancellationToken cancellationToken);
        Task<Either<DockerError?, SystemInfo?>> GetInfoAsync(CancellationToken cancellationToken);
        Task<Either<DockerError?, SystemVersion?>> GetVersionAsync(CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> Ping_Get(CancellationToken cancellationToken);
    }
}