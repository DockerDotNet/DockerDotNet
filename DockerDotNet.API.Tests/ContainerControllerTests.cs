using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text.Json;
using DockerDotNet.Core.Models;
using Xunit.Abstractions;
using Xunit.Sdk;
using DockerDotNet.APIClient.Controllers;
using Shouldly;

namespace DockerDotNet.API.Tests
{
    public class ContainerControllerTests
    {
        ContainerController Controller { get; set; }

        private readonly ITestOutputHelper _output;

        public ContainerControllerTests(ITestOutputHelper testOutputHelper)
        {
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
        }

        public async Task<CreateContainerResponse> CreateContainerAsync()
        {
            CreateContainerQueryParameters queryParameters = new CreateContainerQueryParameters();
            queryParameters.Name = "TestContainers";
            CreateContainerParameters containerParameters = new CreateContainerParameters();
            containerParameters.Image = "alpine:latest";
            CreateContainerResponse containerResponse = await Controller.CreateContainer(queryParameters, containerParameters, CancellationToken.None);
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            containerResponse.ShouldNotBeNull();

            return containerResponse;
        }

        [Fact]
        public async Task StartContainer()
        {
            string id = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";
            var response = await Controller.StartContainer(id, CancellationToken.None);
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);

        }

        [Fact]
        public async Task StopContainer()
        {
            string id = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";
            var response = await Controller.StopContainer(id, CancellationToken.None);
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task RestartContainer()
        {
            string id = "800d3c9395b24e3d98c75655d9720d9997dee27f7ebf1f3aa2ae68caed0e31c4";
            var response = await Controller.RestartContainer(id, CancellationToken.None);
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task KillContainer()
        {
            string id = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";
            var response = await Controller.KillContainer(id, CancellationToken.None);
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task PauseContainer()
        {
            string id = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";
            var response = await Controller.PauseContainer(id, CancellationToken.None);
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task UnpauseContainer()
        {
            string id = "7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb";
            var response = await Controller.UnpauseContainer(id, CancellationToken.None);
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }


        [Fact]
        public async Task GetContainerList()
        {
            ContainersListParameters containersListParameters = new ContainersListParameters();
            containersListParameters.All = true;

            IList<ContainerListResponse> containerListResponse = await Controller.GetContainers(containersListParameters, new CancellationToken());

            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            containerListResponse.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(containerListResponse));
        }

        [Fact]
        public async Task GetContainerInfo()
        {
            ContainerInspectResponse inspectResponse = await Controller.GetContainer("6c2f5ed47d8a384c0bfa417a6d1c32061c3f149d06f04998a584e6fccb7f3b51", new ContainerInspectParameters(), new CancellationToken());
            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
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
            ContainerExecCreateResponse containerExecCreateResponse = await Controller.CreateExec("ce483daa476ed2850c727dbefc32a739af17cac0fba3993d7a322c6c04c5390a", containerExecCreateParameters, new CancellationToken());

            Controller.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            containerExecCreateResponse.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(containerExecCreateResponse));
        }
    }
}