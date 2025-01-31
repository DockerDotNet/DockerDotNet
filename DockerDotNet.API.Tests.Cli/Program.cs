using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DockerDotNet.API.Tests.Cli
{
    internal class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7075") // Your overlay API URL
        };

        static void Main(string[] args)
        {
            Console.ReadKey();
            string execID = string.Empty;
            execID = "8d06ef18b7758507a88c695dec27a8df2a5f5e94507b69ab67b915bd6d2c4a02";
            StartExec(execID);

            Console.ReadKey();
        }

        private async static void StartExec(string execID)
        {
            // /api/exec/{id}/start
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}api/exec/{execID}/start");
            request.Headers.Connection.Add("Upgrade");
            request.Headers.Upgrade.Add(new ProductHeaderValue("hijack"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.Content.Headers.ContentType= new MediaTypeHeaderValue("application/vnd.docker.raw-stream");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to connect: {response.StatusCode}");
                return;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            Console.WriteLine("Connected to container. Type commands:");

            // Read output in a separate task
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    string output = await reader.ReadLineAsync();
                    if (output == null) break;
                    Console.WriteLine(output);
                }
            });

            // Send input to the container
            while (true)
            {
                string command = Console.ReadLine();
                if (string.IsNullOrEmpty(command)) continue;
                await writer.WriteLineAsync(command);
            }
        }
    }
}
