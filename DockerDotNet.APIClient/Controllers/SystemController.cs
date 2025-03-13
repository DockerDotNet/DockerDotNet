using DockerDotNet.Core;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace DockerDotNet.APIClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        private readonly SystemService _systemService;

        public SystemController(SystemService systemService)
        {
            DockerClient = new DockerClient();
            _systemService = systemService;
        }

        [HttpGet]
        [Route("info")]
        public async Task<Core.Models.SystemInfo> GetInfo()
        {
            return await _systemService.GetInfoAsync();
        }

        [HttpGet]
        [Route("version")]
        public async Task<Core.Models.SystemVersion> GetVersion()
        {
            return await _systemService.GetVersionAsync();
        }
    }
}
