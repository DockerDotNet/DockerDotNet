using DockerDotNet.Shared.Models;
using DockerDotNet.Core.Services;

using LanguageExt.Common;

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
using DockerDotNet.Shared.Interfaces;

namespace DockerDotNet.API.Tests
{
    public class SystemServiceTests : DockerTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly ISystemService _systemService;

        public SystemServiceTests(ITestOutputHelper testOutputHelper) : base(Array.Empty<string>())
        {
            _systemService = _host.Services.GetRequiredService<ISystemService>();
            _output = testOutputHelper;
        }

        [Fact]
        public async System.Threading.Tasks.Task GetVersion()
        {
            var response = await _systemService.GetVersionAsync(CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetInfo()
        {
            var response = await _systemService.GetInfoAsync(CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: left => left);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task AuthenticateRegistry()
        {
            AuthConfig authConfig = new AuthConfig();
            var response = await _systemService.AuthenticateRegistry(authConfig, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: result => result);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task Ping_Get()
        {
            var response = await _systemService.Ping_Get(CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: response => response);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetDataUsageInformation()
        {
            var response = await _systemService.GetDataUsageInformation(CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
