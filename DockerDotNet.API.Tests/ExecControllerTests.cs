using DockerDotNet.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Models.Core.Models.APIClient.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace DockerDotNet.API.Tests
{
    public class ExecControllerTests
    {
        ExecController Controller { get; set; }

        private readonly ITestOutputHelper _output;

        public ExecControllerTests(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
            Controller = new ExecController();
            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async void InspectExecInstance()
        {
            ContainerExecInspectResponse inspectResponse = await Controller.InspectExecInstance("eb06d25b2240313da2420624af6f13d6cfe662a57048ef79634fb6d8755c8888", new CancellationToken());
            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            Assert.NotNull(inspectResponse);

            _output.WriteLine(JsonSerializer.Serialize(inspectResponse));
        }

        [Fact]
        public async void StartExecInstance()
        {
            await Controller.StartExecInstance("eb06d25b2240313da2420624af6f13d6cfe662a57048ef79634fb6d8755c8888", new CancellationToken());
            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            //Assert.NotNull(inspectResponse);

            //_output.WriteLine(JsonSerializer.Serialize(inspectResponse));
        }


    }
}
