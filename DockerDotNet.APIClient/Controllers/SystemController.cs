using DockerDotNet.Core;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using System.Threading;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        private readonly SystemService _systemService;

        public SystemController(DockerClient dockerClient, SystemService systemService)
        {
            DockerClient = dockerClient;
            _systemService = systemService;
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> GetInfo(CancellationToken cancellationToken)
        {
            var (status, response, error) = await _systemService.GetInfoAsync(cancellationToken);
            return status ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpGet]
        [Route("version")]
        public async Task<IActionResult> GetVersion(CancellationToken cancellationToken)
        {
            var (status, response, error) = await _systemService.GetVersionAsync(cancellationToken);
            return status ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> AuthenticateRegistry([FromBody] AuthConfig authConfig, CancellationToken cancellationToken)
        {
            var (status, response, error) = await _systemService.AuthenticateRegistry(authConfig, cancellationToken);
            return status ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpGet]
        [Route("ping")]
        public async Task<IActionResult> Ping_Get(CancellationToken cancellationToken)
        {
            var (status, response, error) = await _systemService.Ping_Get(cancellationToken);
            return status ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetSystemDataUsage(CancellationToken cancellationToken)
        {
            var(status, response, error) = await _systemService.GetDataUsageInformation(cancellationToken);
            return status ? Ok(response) : StatusCode((int)error.StatusCode, error.Message);
        }
    }
}
