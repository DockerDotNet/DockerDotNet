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

namespace DockerDotNet.API.Tests
{
    public class ContainerControllerTests
    {
        ContainerController Controller { get; set; }

        private readonly ITestOutputHelper _output;

        string _containerID = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";

        ContainerService _containerService { get; set; }

        public ContainerControllerTests(ITestOutputHelper testOutputHelper)
        {
            DockerClient client = new DockerClient();
            _containerService = new ContainerService(client);

            _output = testOutputHelper;
            Controller = new ContainerController();
            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task CreateContainer()
        {
            CreateContainerResponse container = await CreateContainerAsync();
            container.ID.ShouldNotBeNullOrEmpty();
            _containerID = container.ID;
        }

        public async Task<CreateContainerResponse> CreateContainerAsync()
        {
            CreateContainerQueryParameters queryParameters = new CreateContainerQueryParameters();
            queryParameters.Name = "TestContainers";
            CreateContainerParameters containerParameters = new CreateContainerParameters();
            containerParameters.Image = "nginx:latest";
            CreateContainerResponse containerResponse = await _containerService.CreateContainer(queryParameters, containerParameters, CancellationToken.None);
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            containerResponse.ShouldNotBeNull();

            return containerResponse;
        }

        [Fact]
        public async Task StartContainer()
        {
            var response = await _containerService.StartContainer(_containerID, CancellationToken.None);
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);

        }

        [Fact]
        public async Task StopContainer()
        {
            var response = await _containerService.StopContainer(_containerID, CancellationToken.None);
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task RestartContainer()
        {
            var response = await _containerService.RestartContainer(_containerID, CancellationToken.None);
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task KillContainer()
        {
            var response = await _containerService.KillContainer(_containerID, CancellationToken.None);
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task PauseContainer()
        {
            var response = await _containerService.PauseContainer(_containerID, CancellationToken.None);
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task UnpauseContainer()
        {
            var response = await _containerService.UnpauseContainer(_containerID, CancellationToken.None);
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetContainerList()
        {
            ContainersListParameters containersListParameters = new ContainersListParameters();
            containersListParameters.All = true;

            IList<ContainerListResponse> containerListResponse = await _containerService.GetContainers(containersListParameters, new CancellationToken());
            //IList<ContainerListResponse> containerListResponse = await Controller.GetContainers(containersListParameters, new CancellationToken());

            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            containerListResponse.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(containerListResponse));
        }

        [Fact]
        public async Task GetContainerInfo()
        {
            ContainerInspectResponse inspectResponse = await _containerService.GetContainer(_containerID, new ContainerInspectParameters(), new CancellationToken());
            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            inspectResponse.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(inspectResponse));
        }

        [Fact]
        public async Task CreateExec()
        {
            ContainerExecCreateParameters containerExecCreateParameters = new ContainerExecCreateParameters();
            containerExecCreateParameters.AttachStdout = true;
            containerExecCreateParameters.AttachStderr = true;
            containerExecCreateParameters.AttachStdin = true;
            containerExecCreateParameters.DetachKeys = "ctrl-p,ctrl-q";
            containerExecCreateParameters.Cmd = new List<string>() { "bin/sh"};
            containerExecCreateParameters.Tty = true;
            ContainerExecCreateResponse containerExecCreateResponse = await _containerService.CreateExec(_containerID, containerExecCreateParameters, new CancellationToken());

            //Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            containerExecCreateResponse.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(containerExecCreateResponse));
        }
    }
}