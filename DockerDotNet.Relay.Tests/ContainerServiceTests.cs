using DockerDotNet.Relay.Services;
using DockerDotNet.Shared.Interfaces;
using DockerDotNet.Shared.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace DockerDotNet.Relay.Tests
{
    public class ContainerServiceTests : RelayBase
    {
        private readonly ITestOutputHelper outputHelper;
        private readonly IContainerService containerRelayService;

        #region Constructor

        public ContainerServiceTests(ITestOutputHelper outputHelper) : base(Array.Empty<string>())
        {
            containerRelayService = _host.Services.GetRequiredService<IContainerService>();
            this.outputHelper = outputHelper;
        }

        #endregion

        [Fact]
        public async System.Threading.Tasks.Task GetContainer()
        {
            ContainerInspectParameters parameters = new ContainerInspectParameters();
            var result = await containerRelayService.GetContainer("67a9de123f68f8e7db1965813198aa2da7c0b36c96f9d94058e2295efe75bb47", parameters, CancellationToken.None);

            result.IsRight.ShouldBeTrue();
            var response = result.Match(Right: response => response, Left: _ => null);
            outputHelper.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContainerLogs()
        {
            ContainerLogsParameters parameters = new ContainerLogsParameters();

            var result = await containerRelayService.GetContainerLogs("67a9de123f68f8e7db1965813198aa2da7c0b36c96f9d94058e2295efe75bb47", parameters, CancellationToken.None);

            result.IsRight.ShouldBeTrue();
            var response = result.Match(response => response, Left: _ => null);

            response.ShouldNotBeNull();

            StreamReader streamReader = new StreamReader(response);
            while (!streamReader.EndOfStream)
            {                 
                var line = await streamReader.ReadLineAsync();
                outputHelper.WriteLine(line);
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContainerStats()
        {
            ContainerStatsParameters parameters = new ContainerStatsParameters();

            var result = await containerRelayService.GetContainerStats("67a9de123f68f8e7db1965813198aa2da7c0b36c96f9d94058e2295efe75bb47", parameters, CancellationToken.None);

            result.IsRight.ShouldBeTrue();
            var response = result.Match(response => response, Left: _ => null);

            response.ShouldNotBeNull();

            StreamReader streamReader = new StreamReader(response);
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                outputHelper.WriteLine(line);
            }
        }

    }
}
