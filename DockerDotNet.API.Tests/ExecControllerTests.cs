using DockerDotNet.APIClient.Controllers;
using DockerDotNet.Core.Models;
using DockerDotNet.Core.Services;

using LanguageExt.Pipes;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xunit.Abstractions;

using Task = System.Threading.Tasks.Task;

namespace DockerDotNet.API.Tests
{
    public class ExecControllerTests : DockerTestBase
    {
        private readonly ExecService _execService;

        private readonly ITestOutputHelper _output;

        string containerID = string.Empty;
        string execID = string.Empty;
        
        public ExecControllerTests(ITestOutputHelper testOutputHelper) : base(Array.Empty<string>())
        {
            _output = testOutputHelper;
            _execService = _host.Services.GetRequiredService<ExecService>();
            containerID = "67a9de123f68f8e7db1965813198aa2da7c0b36c96f9d94058e2295efe75bb47";
        }

        [Fact]
        public async Task CreateExecAsync()
        {
            Shared.Models.ExecConfig requestBody = new Shared.Models.ExecConfig();
            requestBody.AttachStdin = true;
            requestBody.AttachStdout = true;
            requestBody.AttachStderr = true;
            requestBody.Cmd = ["sh"];
            requestBody.Tty = false;
            requestBody.DetachKeys = "ctrl-q";

            var response = await _execService.CreateExec(containerID, requestBody, new CancellationToken());

            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: left => null, Right: right => right);
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Shared.Models.ContainerExecCreateResponse>();
            execID = result.ID;
        }

        [Fact]
        public async Task InspectExecInstance()
        {
            await CreateExecAsync();

            var inspectResponse = await _execService.InspectExec(execID, new CancellationToken());

            inspectResponse.IsRight.ShouldBeTrue();
            var result = inspectResponse.Match(Left: left => null, Right: right => right);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }


    }
}
