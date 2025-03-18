using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text.Json;
using DockerDotNet.Core.Models;
using Xunit.Abstractions;
using Xunit.Sdk;
using DockerDotNet.APIClient.Controllers;
using Shouldly;
using DockerDotNet.Core.Services;
using DockerDotNet.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DockerDotNet.API.Tests
{
    public class ContainerControllerTests : DockerTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly ContainerService _containerService;

        string _containerID = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";

        public ContainerControllerTests(ITestOutputHelper testOutputHelper) : base(Array.Empty<string>())
        {
            _containerService = _host.Services.GetRequiredService<ContainerService>();
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
            CreateContainerQueryParameters queryParameters = new CreateContainerQueryParameters();
            queryParameters.Name = "TestContainers";
            ContainerCreateRequest containerParameters = new ContainerCreateRequest();
            containerParameters.Image = "nginx:latest";
            var (success, response, error) = await _containerService.CreateContainer(queryParameters, containerParameters, CancellationToken.None);
            
            success.ShouldBeTrue();
            response.ShouldNotBeNull();

            return response;
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteContainerAsync()
        {
            ContainerDeleteParameters parameters = new ContainerDeleteParameters();

            var (success, _, _) = await _containerService.DeleteContainer(_containerID, parameters, CancellationToken.None);
            success.ShouldBeTrue();
        }

        [Fact]
        public async System.Threading.Tasks.Task StartContainer()
        {
            var (success, _, _) = await _containerService.StartContainer(_containerID, CancellationToken.None);
            success.ShouldBeTrue();

        }

        [Fact]
        public async System.Threading.Tasks.Task StopContainer()
        {
            var (success, _, _) = await _containerService.StopContainer(_containerID, CancellationToken.None);
            success.ShouldBeTrue();
        }

        [Fact]
        public async System.Threading.Tasks.Task RestartContainer()
        {
            var (success, _, _) = await _containerService.RestartContainer(_containerID, CancellationToken.None);
            success.ShouldBeTrue();
        }

        [Fact]
        public async System.Threading.Tasks.Task KillContainer()
        {
            var (success, _, _) = await _containerService.KillContainer(_containerID, CancellationToken.None);
            success.ShouldBeTrue();
        }

        [Fact]
        public async System.Threading.Tasks.Task PauseContainer()
        {
            var (success, _, _) = await _containerService.PauseContainer(_containerID, CancellationToken.None);
            success.ShouldBeTrue();
        }

        [Fact]
        public async System.Threading.Tasks.Task UnpauseContainer()
        {
            var (success, _, _) = await _containerService.UnpauseContainer(_containerID, CancellationToken.None);
            success.ShouldBeTrue();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContainerList()
        {
            ContainersListParameters containersListParameters = new ContainersListParameters();
            containersListParameters.All = true;

            var (success, response, error) = await _containerService.GetContainers(containersListParameters, new CancellationToken());
            success.ShouldBeTrue();
            response.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetContainerInfo()
        {
            var (success, response, error) = await _containerService.GetContainer(_containerID, new ContainerInspectParameters(), new CancellationToken());
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            success.ShouldBeTrue();
            response.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateExec()
        {
            ContainerExecCreateParameters containerExecCreateParameters = new ContainerExecCreateParameters();
            containerExecCreateParameters.AttachStdout = true;
            containerExecCreateParameters.AttachStderr = true;
            containerExecCreateParameters.AttachStdin = true;
            containerExecCreateParameters.DetachKeys = "ctrl-p,ctrl-q";
            containerExecCreateParameters.Cmd = new List<string>() { "bin/sh" };
            containerExecCreateParameters.Tty = true;
            var(success, response, error) = await _containerService.CreateExec(_containerID, containerExecCreateParameters, new CancellationToken());

            success.ShouldBeTrue();
            response.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(response));
        }
    }
}