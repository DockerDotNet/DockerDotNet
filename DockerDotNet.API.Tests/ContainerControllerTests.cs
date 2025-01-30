using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text.Json;
using Models.Core.Models.APIClient.Controllers;
using DockerDotNet.Core.Models;
using Xunit.Abstractions;
using Xunit.Sdk;
using Docker.DotNet.Models;

namespace Models.Core.Models.API.Tests
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
        public async void GetContainerList()
        {
            IList<ContainerListResponse> containerListResponse = await Controller.GetContainers(new ContainersListParameters() { All = true }, new CancellationToken());
            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            Assert.NotNull(containerListResponse);

            _output.WriteLine(JsonSerializer.Serialize(containerListResponse));

        }

        [Fact]
        public async void GetContainerInfo()
        {
            ContainerInspectResponse inspectResponse = await Controller.GetContainer("6c2f5ed47d8a384c0bfa417a6d1c32061c3f149d06f04998a584e6fccb7f3b51", new ContainerInspectParameters(), new CancellationToken());
            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            Assert.NotNull(inspectResponse);
            
            _output.WriteLine(JsonSerializer.Serialize(inspectResponse));
        }

        [Fact]
        public async void CreateExec()
        {
            ContainerExecCreateParameters containerExecCreateParameters = new ContainerExecCreateParameters();
            containerExecCreateParameters.AttachStdout = true;
            containerExecCreateParameters.AttachStderr = true;
            containerExecCreateParameters.AttachStdin = false;
            containerExecCreateParameters.DetachKeys = "ctrl-p,ctrl-q";
            containerExecCreateParameters.Cmd = new List<string>() { "ls"};
            ContainerExecCreateResponse containerExecCreateResponse = await Controller.CreateExec("7733bfa5017ae064b390b3e9428e8dae21c0ffeaf90820c6a9d444fbfc0b08eb", containerExecCreateParameters, new CancellationToken());

            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            Assert.NotNull(containerExecCreateResponse);

            _output.WriteLine(JsonSerializer.Serialize(containerExecCreateResponse));
        }
    }
}