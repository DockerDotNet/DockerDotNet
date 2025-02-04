using DockerDotNet.APIClient.Controllers;
using DockerDotNet.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            ContainerExecInspectResponse inspectResponse = await Controller.InspectExecInstance("940a8e3780e5dd53e2e086a9889b50b6c3a4622677a47fe7d19767bf344cc1cd", new CancellationToken());
            Assert.Equal((int)HttpStatusCode.OK, Controller.Response.StatusCode);
            Assert.NotNull(inspectResponse);

            _output.WriteLine(JsonSerializer.Serialize(inspectResponse));
        }


    }
}
