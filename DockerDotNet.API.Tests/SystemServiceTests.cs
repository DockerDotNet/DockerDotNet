using DockerDotNet.Core.Services;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace DockerDotNet.API.Tests
{
    public class SystemServiceTests : DockerTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly SystemService _systemService;

        public SystemServiceTests(ITestOutputHelper testOutputHelper) : base(Array.Empty<string>())
        {
            _systemService = _host.Services.GetRequiredService<SystemService>();
            _output = testOutputHelper;
        }

        [Fact]
        public async Task GetVersion()
        {
            var(status, response, error) = await _systemService.GetVersionAsync(CancellationToken.None);
            status.ShouldBeTrue();
            response.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async Task GetInfo()
        {
            var (status, response, error) = await _systemService.GetInfoAsync(CancellationToken.None);
            status.ShouldBeTrue();
            response.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(response));
        }
    }
}
