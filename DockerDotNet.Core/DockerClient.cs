using System.IO.Pipes;
using System.Net.Sockets;

namespace DockerDotNet.Core
{
    public class DockerClient
    {
        public Uri? BaseUri { get; set; }

        public Version? Version { get; set; }

        private readonly OSPlatform _operatingSystem;

        private string _versionString = string.Empty;

        public DockerClient() : this(null, null)
        {
        }

        public DockerClient(Uri? baseUri = null, Version? version = null)
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

    }

    public enum OSPlatform
    {
        Undefined = 0,
        Windows = 1,
        Linux = 2,
        MacOS = 3,
    }
}
