using DockerDotNet.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<(bool, VolumeListResponse?, DockerError?)> GetVolumes(VolumesListParameters volumesListParameters, CancellationToken cancellationToken)
        {
            string query = _client.GetQueryString(volumesListParameters);
            return await _client.GetAsync<VolumeListResponse>("volumes", query, cancellationToken);
        }
    }
}
