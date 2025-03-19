using DockerDotNet.Core;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/volumes")]
    [ApiController]
    public class VolumeController : ControllerBase
    {
        private readonly DockerClient _client;
        private VolumeService _volumeService;

        public VolumeController(DockerClient dockerClient, VolumeService volumeService)
        {
            _client = dockerClient;
            _volumeService = volumeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVolumes([FromQuery] VolumesListParameters volumesListParameters, CancellationToken cancellationToken)
        {
            var response = await _volumeService.GetVolumes(volumesListParameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }
    }
}
