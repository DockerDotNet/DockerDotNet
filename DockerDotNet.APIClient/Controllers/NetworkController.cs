using DockerDotNet.Core.Services;
using DockerDotNet.Shared.Interfaces;
using DockerDotNet.Shared.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/networks")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        private readonly INetworkService _networkService;

        public NetworkController(INetworkService networkService)
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

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> InspectNetwork(string id, [FromQuery]NetworkInspectParameters networkInspectParameters, CancellationToken cancellationToken)
        {
            var response = await _networkService.InspectNetwork(id, networkInspectParameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveNetwork(string id, CancellationToken cancellationToken)
        {
            var response = await _networkService.RemoveNetwork(id, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateNetwork([FromBody]NetworkCreateRequest networkCreateRequest, CancellationToken cancellationToken)
        {
            var response = await _networkService.CreateNetwork(networkCreateRequest, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpPost]
        [Route("{id}/connect")]
        public async Task<IActionResult> ConnectContainerToNetwork(string id, NetworkConnectRequest networkConnectRequest, CancellationToken cancellationToken)
        {
            var response = await _networkService.ConnectContainer(id, networkConnectRequest, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpPost]
        [Route("{id}/disconnect")]
        public async Task<IActionResult> DisconnectContainerFromNetwork(string id, NetworkDisconnectRequest networkDisconnectRequest, CancellationToken cancellationToken)
        {
            var response = await _networkService.DisconnectContainer(id, networkDisconnectRequest, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpPost]
        [Route("prune")]
        public async Task<IActionResult> PruneNetwork(NetworkPruneParameters pruneParameters, CancellationToken cancellationToken)
        {
            var response = await _networkService.PruneNetwork(pruneParameters, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }
    }
}
