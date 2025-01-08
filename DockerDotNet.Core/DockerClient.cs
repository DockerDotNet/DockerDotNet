using DockerDotNet.Core.Models;

using Newtonsoft.Json;

using System.IO.Pipes;
using System.Net.Sockets;

namespace DockerDotNet.Core
{
    public class DockerClient
    {
        public Uri? BaseUri { get; set; }

        public System.Version? Version { get; set; }

        private readonly OSPlatform _operatingSystem;

        private string _versionString = string.Empty;

        public DockerClient() : this(null, null)
        {
        }

        public DockerClient(Uri? baseUri = null, System.Version? version = null)
        {
            BaseUri = baseUri;
            Version = version;
            _operatingSystem = GetOperatingSystem();
        }

        public HttpClient GetDockerHttpClient()
        {
            HttpClient httpClient;

            if (BaseUri == null)
                BaseUri = GetLocalUri();

            HttpMessageHandler handler = GetHttpHandler(BaseUri);

            if(Version != null)
            {
                _versionString = $"v{Version}";
                BaseUri = new UriBuilder($"{BaseUri}{_versionString}").Uri;
            }

            httpClient = new HttpClient(handler);
            httpClient.BaseAddress =new UriBuilder("http", BaseUri.Segments.Last()).Uri; ;   

            return httpClient;
        }

        private Uri? GetLocalUri()
        {
            switch (_operatingSystem)
            {
                case OSPlatform.Windows:
                    return new Uri("npipe://./pipe/docker_engine");
                case OSPlatform.Linux:
                    return new Uri("unix:/var/run/docker.sock");
                default:
                    return null;
            }
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

            AuthConfig auth = new AuthConfig()
            {
                ServerAddress = "excellonb2bregsrv.azurecr.io",
                Username = "excellonb2bregsrv",
                Password = "bo3NnGf9UhG=iRFSSoK51Xrlemm5CNUv"
            };

            if(authConfig == null)
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                 resultString = JsonConvert.SerializeObject(auth, serializerSettings);
            }

            byte[] result =  System.Text.Encoding.UTF8.GetBytes(resultString);

            string finalResult = Convert.ToBase64String(result).Replace("/", "_").Replace("+", "-");
            // This is not documented in Docker API but from source code (https://github.com/docker/docker-ce/blob/10e40bd1548f69354a803a15fde1b672cc024b91/components/cli/cli/command/registry.go#L47)
            // and from multiple internet sources it has to be base64-url-safe. 
            // See RFC 4648 Section 5. Padding (=) needs to be kept.

            return finalResult;
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
