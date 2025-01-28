using DockerDotNet.APIClient.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace DockerDotNet.API.Tests
{
    public class ImageControllerTests
    {
        ImageController ImageController { get; set; }

        private readonly ITestOutputHelper _output;
        public ImageControllerTests(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
            ImageController = new ImageController();
            ImageController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async void PullImage_StreamedContent()
        {
            var responseStream = new MemoryStream();
            ImageController.Response.Body = responseStream;  

            await ImageController.PullImage(new Docker.DotNet.Models.ImagesCreateParameters() { FromImage= "excellonb2bregsrv.azurecr.io/businessruleapp:latest" }, null, new CancellationToken());

            Assert.Equal((int)HttpStatusCode.OK, ImageController.Response.StatusCode);

            // Assert and Log
            responseStream.Position = 0;
            using var reader = new StreamReader(responseStream);

            _output.WriteLine("Streaming response content:");
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (line != null)
                {
                    _output.WriteLine(line); // Log each line or chunk
                }
            }
        }

        [Fact]
        public async void GetImageList()
        {
            await ImageController.GetAllImages(new Docker.DotNet.Models.ImagesListParameters(), new CancellationToken());

            Assert.Equal((int)HttpStatusCode.OK, ImageController.Response.StatusCode);
        }

        
    }
}
