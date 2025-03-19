using System.Collections;
using System.IO.Pipes;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using DockerDotNet.Core.Models;

using LanguageExt;

using Task = System.Threading.Tasks.Task;

namespace DockerDotNet.Core
{
    public class DockerClient
    {
        #region Properties

        public Uri? BaseUri { get; set; }

        public System.Version? Version { get; set; }

        private readonly OSPlatform _operatingSystem;

        private string _versionString = string.Empty;

        private JsonSerializerOptions _jsonSerializerOptions;

        #endregion

        #region Constructor

        //public DockerClient(JsonSerializerOptions jsonSerializerOptions)
        //{
        //}

        public DockerClient(JsonSerializerOptions serializerOptions, Uri? baseUri = null, System.Version? version = null)
        {
            BaseUri = baseUri;
            Version = version;
            _operatingSystem = GetOperatingSystem();
            _jsonSerializerOptions = serializerOptions;
        }

        #endregion

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

            authConfig = new AuthConfig()
            {
                Serveraddress = "",
                Username = "",
                Password = ""
            };

            if (authConfig == null)
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

        public HttpRequestMessage PrepareHttpRequest(HttpMethod httpMethod, string endpoint, string queryParameters, Dictionary<string, string>? headers = null, HttpContent? requestBody = null)
        {
            string uriFormat = $"{BaseUri}{endpoint}";

            if (!string.IsNullOrEmpty(queryParameters))
            {
                uriFormat += "?" + queryParameters ;
            }

            Uri requestUri = new UriBuilder(uriFormat).Uri;
            
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri);

            AddHeadersToRequest(httpRequestMessage, headers);

            if (requestBody != null)
            {
                httpRequestMessage.Content = requestBody;
            }

            return httpRequestMessage;
        }

        public string GetQueryString<T>(T dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var keyValuePairs = new List<string>();
            foreach (var property in properties)
            {
                var value = property.GetValue(dto);
                if (value == null)
                    continue;

                // Check if the property has a JsonPropertyName attribute
                var jsonPropertyNameAttribute = property
                    .GetCustomAttribute<JsonPropertyNameAttribute>();

                string propertyName = jsonPropertyNameAttribute?.Name ?? property.Name;
                string encodedKey = HttpUtility.UrlEncode(propertyName);
                string encodedValue = string.Empty;

                //if (property.PropertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                if (value is IDictionary)
                {
                    encodedValue = GetMapQuery(value as IDictionary);
                }
                else
                {
                    // commenting this because it gives error for image name string
                    //encodedValue = HttpUtility.UrlEncode(value.ToString());
                    encodedValue = value?.ToString();
                }
                keyValuePairs.Add($"{Uri.EscapeUriString(encodedKey)}={Uri.EscapeDataString(encodedValue)}");
            }
            return string.Join("&", keyValuePairs);
        }

        private string GetMapQuery(IDictionary dictionary)
        {
            return System.Text.Json.JsonSerializer.Serialize(dictionary);
        }

        public async Task<Either<DockerError?, T?>> GetAsync<T>(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? requestBody = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = PrepareHttpRequest(HttpMethod.Get, endpoint, queryParameters, headers, requestBody);
                        
            HttpResponseMessage response = await client.SendAsync(requestMessage, cancellationToken);

            return await ProcessResponse<T>(response, cancellationToken);
        }

        public async Task<Either<DockerError?, T?>> PostAsync<T>(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? body = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = PrepareHttpRequest(HttpMethod.Post, endpoint, queryParameters, headers, body);

            HttpResponseMessage response = await client.SendAsync(requestMessage, cancellationToken);

            return await ProcessResponse<T>(response, cancellationToken);
        }

        public async Task<Either<DockerError?, T?>> DeleteAsync<T>(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken,
            Dictionary<string, string>? headers = null,
            HttpContent? body = null)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = PrepareHttpRequest(HttpMethod.Delete, endpoint, queryParameters, headers, body);

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage, cancellationToken);

            return await ProcessResponse<T>(responseMessage, cancellationToken);
        }

        public async Task<(bool, Stream?, string, DockerError?)> GetStreamAsync(
            string endpoint,
            string queryParameters,
            CancellationToken cancellationToken)
        {
            var client = GetDockerHttpClient();

            HttpRequestMessage requestMessage = PrepareHttpRequest(HttpMethod.Get, endpoint, queryParameters);

            HttpResponseMessage response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return await ProcessStreamResponse<Stream>(response, cancellationToken);
        }

        private async Task<(bool, Stream?, string, DockerError?)> ProcessStreamResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            string contentType = response.Content.Headers.ContentType.ToString();

            if(response.IsSuccessStatusCode)
            {
                Stream stream = await response.Content.ReadAsStreamAsync();
                return (true, stream, contentType, null);
            }

            if (contentType == "application/json")
            {
                var errorContent = await response.Content.ReadFromJsonAsync<DockerErrorResponse>(cancellationToken);
                return (false, null, contentType, new DockerError(response.StatusCode, errorContent.Message));
            }

            return (false, default,  default, new DockerError(response.StatusCode, "Unable to handle string"));
            
        }

        private async Task<Either<DockerError?, T?>> ProcessResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<DockerErrorResponse>(cancellationToken);
                return new DockerError(response.StatusCode, errorContent.Message);
            }

            object content;
            if(typeof(T) == typeof(string))
            {
                content = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            //else if(typeof(T) == typeof(Stream))
            //{
            //    content = await response.
            //}
            else
            {
                JsonSerializerOptions options = new JsonSerializerOptions(_jsonSerializerOptions);
                content = await response.Content.ReadFromJsonAsync<T>(options, cancellationToken);
            }
            return (T)content;
        }

        private void AddHeadersToRequest(HttpRequestMessage requestMessage, Dictionary<string, string>? headers)
        {
            if (headers == null) return;

            foreach (var header in headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
    }

    public record DockerErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class DockerError
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }

        public DockerError(HttpStatusCode statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
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

