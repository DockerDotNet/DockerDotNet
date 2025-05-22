using DockerDotNet.Core.Models;

using LanguageExt;

namespace DockerDotNet.Core.Interfaces
{
    public interface IVolumeService
    {
        Task<Either<DockerError?, Volume?>> CreateVolume(VolumeCreateOptions volumeCreateOptions, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> DeleteVolume(string name, VolumeDeleteParameters volumeDeleteParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, VolumeListResponse?>> GetVolumes(VolumesListParameters volumesListParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, Volume?>> InspectVolume(string name, CancellationToken cancellationToken);
        Task<Either<DockerError?, VolumePruneResponse?>> PruneVolumes(VolumePruneParameters volumePruneParameters, CancellationToken cancellationToken);
    }
}