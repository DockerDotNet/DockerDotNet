using DockerDotNet.Shared.Models;
using LanguageExt;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace DockerDotNet.Shared.Helpers
{
    public class HttpClientHelper
    {
        #region Declarations

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IHttpClientFactory _clientFactory;

        #endregion

        #region Constructor

        public HttpClientHelper(IHttpClientFactory clientFactory, JsonSerializerOptions jsonSerializerOptions)
        {
            _clientFactory = clientFactory;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        #endregion

        #region HTTP Methods

        public async Task<Either<DockerError?, T?>> SendRequestAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken, HttpClient? client = null)
        {
            client ??= _clientFactory.CreateClient();

            HttpResponseMessage responseMessage = await client.SendAsync(request, cancellationToken);

            return await ProcessResponse<T>(responseMessage, cancellationToken);
        }

        public async Task<Either<DockerError?, Stream?>> SendStreamRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken, HttpClient? client = null)
        {
            client ??= _clientFactory.CreateClient();

            HttpResponseMessage responseMessage = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            return await ProcessStreamResponse<Stream?>(responseMessage, cancellationToken);
        }

        #endregion

        #region Helper Methods


        public HttpRequestMessage PrepareHttpRequestMessage(Uri baseAddress, HttpMethod httpMethod, string endpoint, string queryParameters, Dictionary<string, string>? headers = null, HttpContent? requestBody = null)
        {
            string uriFormat = $"{baseAddress}{endpoint}";

            // Add query parameters
            if (!string.IsNullOrEmpty(queryParameters))
            {
                uriFormat += "?" + queryParameters;
            }

            Uri requestUri = new UriBuilder(uriFormat).Uri;

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri);

            // Add headers
            AddHeadersToRequest(httpRequestMessage, headers);

            // Add body
            if (requestBody != null)
            {
                httpRequestMessage.Content = requestBody;
            }

            return httpRequestMessage;
        }

        private void AddHeadersToRequest(HttpRequestMessage requestMessage, Dictionary<string, string>? headers)
        {
            if (headers == null) return;

            foreach (var header in headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        public async Task<Either<DockerError?, T?>> ProcessResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken);
                return new DockerError(response.StatusCode, errorContent.Message);
            }

            object? content;
            if (typeof(T) == typeof(string))
            {
                content = await response.Content.ReadAsStringAsync(cancellationToken);
            }
            else
            {
                JsonSerializerOptions options = new JsonSerializerOptions(_jsonSerializerOptions);
                content = await response.Content.ReadFromJsonAsync<T>(options, cancellationToken);
            }
            return (T?)content;
        }

        public async Task<Either<DockerError?, Stream?>> ProcessStreamResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            string? contentType = response.Content.Headers.ContentType?.ToString();

            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.SwitchingProtocols)
            {
                Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                return stream;
            }

            if (contentType == "application/json")
            {
                var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken);
                return new DockerError(response.StatusCode, errorContent.Message);
            }

            return new DockerError(response.StatusCode, "Unable to handle string");

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
                string encodedValue;

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
                keyValuePairs.Add($"{Uri.EscapeDataString(encodedKey)}={Uri.EscapeDataString(encodedValue)}");
            }
            return string.Join("&", keyValuePairs);
        }

        private string GetMapQuery(IDictionary dictionary)
        {
            return System.Text.Json.JsonSerializer.Serialize(dictionary);
        }

        #endregion
    }
}
