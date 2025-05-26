using DockerDotNet.Core;
using DockerDotNet.Shared.Models;
using DockerDotNet.Core.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading;
using System.Xml.Linq;

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

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateVolume([FromBody]VolumeCreateOptions volumeCreateOptions, CancellationToken cancellationToken)
        {
            var response = await _volumeService.CreateVolume(volumeCreateOptions, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> InspectVolume(string name, CancellationToken cancellationToken)
        {
            var response = await _volumeService.InspectVolume(name, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> DeleteVolume(string name, [FromQuery]VolumeDeleteParameters volumeDeleteParameters, CancellationToken cancellationToken)
        {
            var response = await _volumeService.DeleteVolume(name, volumeDeleteParameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpPost]
        [Route("prune")]
        public async Task<IActionResult> PruneVolumes([FromQuery]VolumePruneParameters volumePruneParameters, CancellationToken cancellationToken)
        {
            var response = await _volumeService.PruneVolumes(volumePruneParameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }
    }
}
