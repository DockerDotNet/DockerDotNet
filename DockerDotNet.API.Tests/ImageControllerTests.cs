using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using DockerDotNet.Core.Models;
using Xunit.Abstractions;
using DockerDotNet.APIClient.Controllers;
using DockerDotNet.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Text.Json;

namespace DockerDotNet.API.Tests
{
    public class ImageControllerTests : DockerTestBase
    {
        ImageController ImageController { get; set; }

        private readonly ImageService _imageService;

        private readonly ITestOutputHelper _output;
        public ImageControllerTests(ITestOutputHelper testOutputHelper) : base(Array.Empty<string>())
        {
            _imageService = _host.Services.GetRequiredService<ImageService>(); 
            _output = testOutputHelper;
        }

        [Fact]
        public async void PullImage_StreamedContent()
        {
            // TODO: Write this test properly after implementing streaming abstraction
            var responseStream = new MemoryStream();
            ImageController.Response.Body = responseStream;

            await ImageController.PullImage(new ImagesCreateParameters() { FromImage = "excellonb2bregsrv.azurecr.io/businessruleapp:latest" }, null, new CancellationToken());

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
        public async System.Threading.Tasks.Task GetImageList()
        {
            var (success, response, _) = await _imageService.GetImages(new ImagesListParameters(), new CancellationToken());
            success.ShouldBeTrue();
            response.ShouldNotBeNull();
            _output.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetImage()
        {
            string imageName = "04bf2359fb0d7f18a2c98856d506051ab600624d686ba2114352fd85c28004cc";
            var (success, response, _) = await _imageService.GetImage(imageName, new CancellationToken());
            success.ShouldBeTrue();
            response.ShouldNotBeNull();
            _output.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetImageHistory()
        {
            string imageName = "04bf2359fb0d7f18a2c98856d506051ab600624d686ba2114352fd85c28004cc";
            var (success, response, _) = await _imageService.GetImageHistory(imageName, new CancellationToken());
            success.ShouldBeTrue();
            response.ShouldNotBeNull();
            _output.WriteLine(JsonSerializer.Serialize(response));
        }

        [Fact]
        public async System.Threading.Tasks.Task TagImage()
        {
            string imageName = "04bf2359fb0d7f18a2c98856d506051ab600624d686ba2114352fd85c28004cc";
            
            ImageTagParameters parameters = new ImageTagParameters();
            parameters.Repository = "";
            parameters.Tag = "";

            var (success, response, _) = await _imageService.TagImage(imageName, parameters, new CancellationToken());
            
            success.ShouldBeTrue();
            response.ShouldNotBeNull();
            
            _output.WriteLine(JsonSerializer.Serialize(response));
        }
    }
}
