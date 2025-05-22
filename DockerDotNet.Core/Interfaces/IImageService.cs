using DockerDotNet.Core.Models;

using LanguageExt;

using System.Net.WebSockets;

namespace DockerDotNet.Core.Interfaces
{
    public interface IImageService
    {
        Task<Either<DockerError?, BuildPruneResponse?>> BuildPrune(BuildPruneParameters buildPruneParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, Stream?>> CreateImage(ImagesCreateParameters parameters, WebSocket webSocket, CancellationToken cancellationToken);
        Task<Either<DockerError?, ImageDeleteResponseItem?>> DeleteImage(string name, ImageDeleteParameters parameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, ImageInspect?>> GetImage(string name, CancellationToken cancellationToken);
        Task<Either<DockerError?, List<HistoryResponseItem>?>> GetImageHistory(string name, CancellationToken cancellationToken);
        Task<Either<DockerError?, IList<ImageSummary>?>> GetImages(ImagesListParameters imagesListParameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, List<ImageSearchResponseItem>?>> ImageSearch(ImageSearchParameters parameters, CancellationToken cancellationToken);
        Task<Either<DockerError?, string?>> TagImage(string name, ImageTagParameters parameters, CancellationToken cancellationToken);
    }
}