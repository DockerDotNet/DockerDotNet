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
    public class VolumeService
    {
        private readonly DockerClient _client;

        public VolumeService(DockerClient dockerClient)
        {
            _client = dockerClient;
        }

        public async Task<Either<DockerError?, VolumeListResponse?>> GetVolumes(VolumesListParameters volumesListParameters, CancellationToken cancellationToken)
        {
            string query = _client.GetQueryString(volumesListParameters);
            return await _client.GetAsync<VolumeListResponse>("volumes", query, cancellationToken);
        }

        public async Task<Either<DockerError?, Volume?>> CreateVolume(VolumeCreateOptions volumeCreateOptions, CancellationToken cancellationToken)
        {
            return await _client.PostAsync<Volume>("volumes/create", string.Empty, cancellationToken, body: JsonContent.Create(volumeCreateOptions));
        }

        public async Task<Either<DockerError?,Volume?>> InspectVolume(string name, CancellationToken cancellationToken)
        {
            return await _client.GetAsync<Volume>($"volumes/{name}", string.Empty, cancellationToken);
        }

        public async Task<Either<DockerError?, string?>> DeleteVolume(string name, VolumeDeleteParameters volumeDeleteParameters, CancellationToken cancellationToken)
        {
            string query = _client.GetQueryString(volumeDeleteParameters);
            return await _client.DeleteAsync<string>($"volumes/{name}", query, cancellationToken);
        }

        public async Task<Either<DockerError?, VolumePruneResponse?>> PruneVolumes(VolumePruneParameters volumePruneParameters, CancellationToken cancellationToken)
        {
            string query = _client.GetQueryString(volumePruneParameters);
            return await _client.PostAsync<VolumePruneResponse>("volumes/prune", query, cancellationToken);
        }
    }
}
