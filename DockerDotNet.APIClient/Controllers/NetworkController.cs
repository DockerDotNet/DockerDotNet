using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/networks")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly NetworkService _networkService;

        public NetworkController(NetworkService networkService)
        {
            this._networkService = networkService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> ListNetworks([FromQuery] NetworkListParameters networkListParameters, CancellationToken cancellationToken)
        {
            var response = await _networkService.ListNetworks(networkListParameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

    }
}
