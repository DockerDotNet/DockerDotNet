using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text.Json;
using Xunit.Abstractions;
using Xunit.Sdk;
using DockerDotNet.APIClient.Controllers;
using Shouldly;
using DockerDotNet.Core.Services;
using DockerDotNet.Shared.Models;
using DockerDotNet.Core;
using Microsoft.Extensions.DependencyInjection;
using DockerDotNet.Shared.Interfaces;

namespace DockerDotNet.API.Tests
{
    public class ContainerControllerTests : DockerTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly IContainerService _containerService;

        string _containerID = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";

        public ContainerControllerTests(ITestOutputHelper testOutputHelper) : base(Array.Empty<string>())
        {
            _containerService = _host.Services.GetRequiredService<IContainerService>();
            _output = testOutputHelper;
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateContainer()
        {
            ContainerCreateResponse container = await CreateContainerAsync();
            container.Id.ShouldNotBeNullOrEmpty();
            _containerID = container.Id;
        }

        public async Task<ContainerCreateResponse> CreateContainerAsync()
        {
            ContainerCreateParameters queryParameters = new ContainerCreateParameters();
            queryParameters.Name = "TestContainers";
            ContainerCreateRequest containerParameters = new ContainerCreateRequest();
            containerParameters.Image = "nginx:latest";
            var response = await _containerService.CreateContainer(queryParameters, containerParameters, CancellationToken.None);

            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: left => null, Right: right => right);
            result.ShouldNotBeNull();
            return result;
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteContainerAsync()
        {
            ContainerDeleteParameters parameters = new ContainerDeleteParameters();

            var response = await _containerService.DeleteContainer(_containerID, parameters, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: left => null, Right: right => right);
            result.ShouldNotBeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task StartContainer()
        {
            var response = await _containerService.StartContainer(_containerID, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: left => null, Right: right => right);
            result.ShouldNotBeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task StopContainer()
        {
            var response = await _containerService.StopContainer(_containerID, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task RestartContainer()
        {
            var response = await _containerService.RestartContainer(_containerID, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task KillContainer()
        {
            var response = await _containerService.KillContainer(_containerID, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task PauseContainer()
        {
            var response = await _containerService.PauseContainer(_containerID, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task UnpauseContainer()
        {
            var response = await _containerService.UnpauseContainer(_containerID, CancellationToken.None);
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContainerList()
        {
            ContainersListParameters containersListParameters = new ContainersListParameters();
            containersListParameters.All = true;

            var response = await _containerService.GetContainers(containersListParameters, new CancellationToken());
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContainerInfo()
        {
            var response = await _containerService.GetContainer(_containerID, new ContainerInspectParameters(), new CancellationToken());
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: response => response);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        //[Fact]
        //public async System.Threading.Tasks.Task CreateExec()
        //{
        //    ContainerExecCreateParameters containerExecCreateParameters = new ContainerExecCreateParameters();
        //    containerExecCreateParameters.AttachStdout = true;
        //    containerExecCreateParameters.AttachStderr = true;
        //    containerExecCreateParameters.AttachStdin = true;
        //    containerExecCreateParameters.DetachKeys = "ctrl-p,ctrl-q";
        //    containerExecCreateParameters.Cmd = new List<string>() { "bin/sh" };
        //    containerExecCreateParameters.Tty = true;
        //    var response = await _containerService.CreateExec(_containerID, containerExecCreateParameters, new CancellationToken());
        //    response.IsRight.ShouldBeTrue();
        //    var result = response.Match(Left: null, Right: response => response);
        //    result.ShouldNotBeNull();

        //    _output.WriteLine(JsonSerializer.Serialize(result));
        //}
    }
}