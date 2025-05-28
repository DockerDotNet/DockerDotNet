using DockerDotNet.Shared.Interfaces;
using DockerDotNet.Shared.Models;

using Microsoft.AspNetCore.Mvc;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;

        public SystemController(ISystemService systemService)
        {
            _systemService = systemService;
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> GetInfo(CancellationToken cancellationToken)
        {
            var response = await _systemService.GetInfoAsync(cancellationToken);
            return response.Match(
                Left: error => StatusCode((int)error.StatusCode, error.Message),
                Right: result => Ok(result)
                );
        }

        [HttpGet]
        [Route("version")]
        public async Task<IActionResult> GetVersion(CancellationToken cancellationToken)
        {
            var response = await _systemService.GetVersionAsync(cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> AuthenticateRegistry([FromBody] AuthConfig authConfig, CancellationToken cancellationToken)
        {
            var response = await _systemService.AuthenticateRegistry(authConfig, cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpGet]
        [Route("ping")]
        public async Task<IActionResult> Ping_Get(CancellationToken cancellationToken)
        {
            var response = await _systemService.Ping_Get(cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetSystemDataUsage(CancellationToken cancellationToken)
        {
            var response = await _systemService.GetDataUsageInformation(cancellationToken);
            return response.Match(Left: error => StatusCode((int)error.StatusCode, error.Message), Right: result => Ok(result));
        }
    }
}
