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
            var (success, response, error) = await _volumeService.GetVolumes(volumesListParameters, cancellationToken);
            return success ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }
    }
}
