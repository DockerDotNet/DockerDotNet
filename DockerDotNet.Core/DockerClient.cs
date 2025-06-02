using DockerDotNet.Shared.Helpers;
using DockerDotNet.Shared.Models;

using LanguageExt;

using System.Collections;
using System.IO.Pipes;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace DockerDotNet.Core
{
    public class DockerClient
    {
        #region Properties

        public Uri? BaseUri { get; set; }

        public System.Version? Version { get; set; }

        private readonly OSPlatform _operatingSystem;

        private string _versionString = string.Empty;
        private readonly HttpClientHelper clientHelper;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        #endregion

        #region Constructor

        //public DockerClient(JsonSerializerOptions jsonSerializerOptions)
        //{
        //}

        public DockerClient(HttpClientHelper clientHelper, JsonSerializerOptions serializerOptions, Uri? baseUri = null, System.Version? version = null)
        {
            BaseUri = baseUri;
            Version = version;
            _operatingSystem = GetOperatingSystem();
            this.clientHelper = clientHelper;
            _jsonSerializerOptions = serializerOptions;
        }

        #endregion

        public HttpClient GetDockerHttpClient()
        {
            HttpClient httpClient;

            //if (BaseUri == null)
                BaseUri = GetLocalUri();

            HttpMessageHandler handler = GetHttpHandler(BaseUri);

            if(Version != null)
            {
                _versionString = $"v{Version}";
                BaseUri = new UriBuilder($"{BaseUri}{_versionString}").Uri;
            }

            httpClient = new HttpClient(handler);
            httpClient.BaseAddress =new UriBuilder("http", BaseUri.Segments.Last()).Uri;   
            BaseUri = httpClient.BaseAddress;

            return httpClient;
        }

        private Uri? GetLocalUri()
        {
            return _operatingSystem switch
            {
                OSPlatform.Windows => new Uri("npipe://./pipe/docker_engine"),
                OSPlatform.Linux => new Uri("unix:/var/run/docker.sock"),
                _ => null,
            };
        }

        private OSPlatform GetOperatingSystem()
        {
            if(OperatingSystem.IsWindows())
                return OSPlatform.Windows;
            else if(OperatingSystem.IsLinux())
                return OSPlatform.Linux;
            else if(OperatingSystem.IsMacOS())
                return OSPlatform.MacOS;
            else 
                return OSPlatform.Undefined;
        }

        private HttpMessageHandler GetHttpHandler(Uri baseUri)
        {
            HttpMessageHandler httpHandler;

            switch (_operatingSystem)
            {
                case OSPlatform.Windows:
                    {
                        string pipeName = baseUri.Segments.Last();
                        httpHandler = new SocketsHttpHandler
                        {
                            // Called to open a new connection
                            ConnectCallback = async (ctx, ct) =>
                            {
                                // Configure the named pipe stream
                                var pipeClientStream = new NamedPipeClientStream(
                                    serverName: ".", // 👈 this machine
                                    pipeName: pipeName, // 👈 
                                    PipeDirection.InOut, // We want a duplex stream 
                                    PipeOptions.Asynchronous); // Always go async

                                // Connect to the server!
                                await pipeClientStream.ConnectAsync(ct);

                                return pipeClientStream;
                            }
                        };
                    }
                    break;
                case OSPlatform.Linux:
                    {
                        string pipeString = baseUri.LocalPath;
                        httpHandler = new SocketsHttpHandler
                        {
                            ConnectCallback = async (context, cancellationToken) =>
                            {
                                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                                var endpoint = new UnixDomainSocketEndPoint(pipeString);
                                await socket.ConnectAsync(endpoint, cancellationToken);
                                return new NetworkStream(socket, ownsSocket: true);
                            }
                        };
                    }
                    break;
                default:
                    httpHandler = new HttpClientHandler();
                    break;
            }  

            return httpHandler;
        }

        public Dictionary<string, string> GetRegistryAuthHeaders(AuthConfig authConfig)
        {
            return new Dictionary<string, string>
            {
                {
                    "X-Registry-Auth", GetRegistryAuthCredentialsString(authConfig)
                }
            };
        }

        private string GetRegistryAuthCredentialsString(AuthConfig authConfig)
        {
            string resultString = string.Empty;

            if (authConfig != null)
            {
                JsonSerializerOptions serializerSettings = new JsonSerializerOptions();
                serializerSettings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                 resultString = JsonSerializer.Serialize(authConfig, serializerSettings);
            }

            byte[] result =  System.Text.Encoding.UTF8.GetBytes(resultString);

            string finalResult = Convert.ToBase64String(result).Replace("/", "_").Replace("+", "-");
            // This is not documented in Docker API but from source code (https://github.com/docker/docker-ce/blob/10e40bd1548f69354a803a15fde1b672cc024b91/components/cli/cli/command/registry.go#L47)
            // and from multiple internet sources it has to be base64-url-safe. 
            // See RFC 4648 Section 5. Padding (=) needs to be kept.

            return finalResult;
        }

        public string GetQueryString<T>(T dto)
        {
            return clientHelper.GetQueryString<T>(dto);
        }

        public async Task<Either<DockerError?, T?>> GetAsync<T>(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? requestBody = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = clientHelper.PrepareHttpRequestMessage(BaseUri, HttpMethod.Get, endpoint, queryParameters, headers, requestBody);

            return await clientHelper.SendRequestAsync<T>(requestMessage, cancellationToken, client);
        }

        public async Task<Either<DockerError?, T?>> PostAsync<T>(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? body = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = clientHelper.PrepareHttpRequestMessage(BaseUri, HttpMethod.Post, endpoint, queryParameters, headers, body);

            return await clientHelper.SendRequestAsync<T>(requestMessage, cancellationToken, client);
        }

        public async Task<Either<DockerError?, T?>> DeleteAsync<T>(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? body = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = clientHelper.PrepareHttpRequestMessage(BaseUri, HttpMethod.Delete, endpoint, queryParameters, headers, body);

            return await clientHelper.SendRequestAsync<T>(requestMessage, cancellationToken, client);
        }

        public async Task<Either<DockerError?, Stream?>> GetStreamAsync(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? body = null)
        {
            var client = GetDockerHttpClient();
            
            HttpRequestMessage requestMessage = clientHelper.PrepareHttpRequestMessage(BaseUri, HttpMethod.Get, endpoint, queryParameters, headers: headers, requestBody: body);

            return await clientHelper.SendStreamRequestAsync(requestMessage, cancellationToken, client);
        }

        public async Task<Either<DockerError?, Stream?>> PostHijackedStreamAsync(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? body = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = clientHelper.PrepareHttpRequestMessage(BaseUri, HttpMethod.Post, endpoint, queryParameters, headers: headers, requestBody: body);

            requestMessage.Headers.Connection.Add("Upgrade");
            requestMessage.Headers.Upgrade.Add(new ProductHeaderValue("hijack"));

            return await clientHelper.SendStreamRequestAsync(requestMessage, cancellationToken, client);
        }

        public async Task<Either<DockerError?, Stream?>> PostStreamAsync(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? body = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = clientHelper.PrepareHttpRequestMessage(BaseUri, HttpMethod.Post, endpoint, queryParameters, headers: headers, requestBody: body);

            return await clientHelper.SendStreamRequestAsync(requestMessage, cancellationToken, client);
        }
    }

    public enum OSPlatform
    {
        Undefined = 0,
        Windows = 1,
        Linux = 2,
        MacOS = 3,
    }
}

