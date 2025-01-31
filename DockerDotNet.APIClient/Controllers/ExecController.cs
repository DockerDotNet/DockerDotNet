using DockerDotNet.Core;
using DockerDotNet.Core.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;

using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using System.Net.Sockets;
using System.Net;

namespace Models.Core.Models.APIClient.Controllers
{
    [Route("api/exec")]
    [ApiController]
    public class ExecController : ControllerBase
    {
        DockerClient DockerClient { get; set; }

        public ExecController()
        {
            DockerClient = new DockerClient();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ContainerExecInspectResponse> InspectExecInstance(string id, CancellationToken cancellationToken)
        {
            using HttpClient httpClient = DockerClient.GetDockerHttpClient();
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Get, $"exec/{id}/json", string.Empty);

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            ContainerExecInspectResponse? responseContent = await httpResponseMessage.Content.ReadFromJsonAsync<ContainerExecInspectResponse>(jsonSerializerOptions, cancellationToken);

            return responseContent;
        }

        [HttpPost]
        [Route("{id}/start")]
        public async Task StartExecInstance(string id, CancellationToken cancellationToken)
        {
            HttpClient httpClient = DockerClient.GetDockerHttpClient();
            
            HttpRequestMessage requestMessage = DockerClient.PrepareHttpRequest(HttpMethod.Post, $"exec/{id}/start", string.Empty);
            requestMessage.Version = HttpVersion.Version11; ;
            requestMessage.Headers.Connection.Add("Upgrade");
            requestMessage.Headers.Upgrade.Add(new ProductHeaderValue("hijack"));
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new { Detach = false, Tty = true }), Encoding.UTF8, "application/json");
            //HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            Response.Headers["Content-Type"] = "application/vnd.docker.raw-stream";
            Response.Headers["Transfer-Encoding"] = "chunked";
            HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
            await Response.Body.FlushAsync();

            // Read the chunked response stream properly
            using var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            //return (Stream)
            using var outputStream = Response.Body;
            StreamReader reader = new StreamReader(stream);

            char[] buffer = new char[1024];
            int bytesRead;

            while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                // Convert the read data to bytes and write to response
                byte[] bytesToWrite = Encoding.UTF8.GetBytes(buffer, 0, bytesRead);
                await outputStream.WriteAsync(bytesToWrite, 0, bytesToWrite.Length);
                await outputStream.FlushAsync();
            }
        }
    }
}
