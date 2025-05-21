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
    public class ImageServiceTests : DockerTestBase
    {
        private readonly ImageService _imageService;

        private readonly ITestOutputHelper _output;

        private string imageName = string.Empty;

        public ImageServiceTests(ITestOutputHelper testOutputHelper) : base(Array.Empty<string>())
        {
            _imageService = _host.Services.GetRequiredService<ImageService>(); 
            _output = testOutputHelper;
            imageName = "1b7c17f650602d97a10724d796f45f0b5250d47ee5ba02f28de89f8a1531f3ce";
        }

        //[Fact]
        //public async void PullImage_StreamedContent()
        //{
        //    // TODO: Write this test properly after implementing streaming abstraction
        //    var responseStream = new MemoryStream();
        //    ImageController.Response.Body = responseStream;

        //    //await ImageController.CreateImage(new ImagesCreateParameters() { FromImage = "excellonb2bregsrv.azurecr.io/businessruleapp:latest" }, null, new CancellationToken());

        //    Assert.Equal((int)HttpStatusCode.OK, ImageController.Response.StatusCode);

        //    // Assert and Log
        //    responseStream.Position = 0;
        //    using var reader = new StreamReader(responseStream);

        //    _output.WriteLine("Streaming response content:");
        //    while (!reader.EndOfStream)
        //    {
        //        var line = await reader.ReadLineAsync();
        //        if (line != null)
        //        {
        //            _output.WriteLine(line); // Log each line or chunk
        //        }
        //    }
        //}

        [Fact]
        public async System.Threading.Tasks.Task GetImageList()
        {
            var response = await _imageService.GetImages(new ImagesListParameters(), new CancellationToken());
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: result => result);
            result.ShouldNotBeNull();
            //result.ShouldBeOfType<IList<ImageSummary>>();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetImage()
        {
            var response = await _imageService.GetImage(imageName, new CancellationToken());
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();
            
            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task GetImageHistory()
        {
            var response = await _imageService.GetImageHistory(imageName, new CancellationToken());
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: result => result);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task TagImage()
        {
            ImageTagParameters parameters = new ImageTagParameters
            {
                Repository = "",
                Tag = ""
            };

            var response = await _imageService.TagImage(imageName, parameters, new CancellationToken());
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();
            
            _output.WriteLine(JsonSerializer.Serialize(result));
        }

        [Fact]
        public async System.Threading.Tasks.Task SearchImage()
        {
            ImageSearchParameters imageSearchParameters = new ImageSearchParameters();
            imageSearchParameters.Term = "alpine";
            imageSearchParameters.Limit = 10;

            var response = await _imageService.ImageSearch(imageSearchParameters, new CancellationToken());
            response.IsRight.ShouldBeTrue();
            var result = response.Match(Left: null, Right: right => right);
            result.ShouldNotBeNull();

            _output.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
