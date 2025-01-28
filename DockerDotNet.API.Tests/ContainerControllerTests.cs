using Docker.DotNet.Models;

using DockerDotNet.APIClient.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text.Json;

using Xunit.Abstractions;
using Xunit.Sdk;

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
        public async void GetContainerList()
        {
            IList<ContainerListResponse> containerListResponse = await Controller.GetContainers(new Docker.DotNet.Models.ContainersListParameters() { All = true }, new CancellationToken());
            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            Assert.NotNull(containerListResponse);

            _output.WriteLine(JsonSerializer.Serialize(containerListResponse));

        }

        [Fact]
        public async void GetContainerInfo()
        {
            ContainerInspectResponse inspectResponse = await Controller.GetContainer("6c2f5ed47d8a384c0bfa417a6d1c32061c3f149d06f04998a584e6fccb7f3b51", new Docker.DotNet.Models.ContainerInspectParameters(), new CancellationToken());
            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            Assert.NotNull(inspectResponse);
            
            _output.WriteLine(JsonSerializer.Serialize(inspectResponse));
        }

    }
}