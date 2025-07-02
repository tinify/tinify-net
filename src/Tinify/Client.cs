using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace TinifyAPI
{
    using Method = HttpMethod;

    public sealed class Client : IDisposable
    {
        internal sealed class ErrorData
        {
            [JsonPropertyName("message")]
            public string Message { get; init; }
            [JsonPropertyName("error")]
            public string Error { get; init; }
        }

        public static readonly Uri ApiEndpoint = new("https://api.tinify.com");

        public static readonly short RetryCount = 1;
        public static ushort RetryDelay { get; internal set; }= 500;

        public static readonly string UserAgent = Internal.Platform.UserAgent;

        private readonly HttpClient _client;

        public Client(string key, string appIdentifier = null, string proxy = null)
        {
            var handler = new HttpClientHandler();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // TLS is extremely spotty and differs per version on MacOS
                ServerCertificateCustomValidationCallback = Internal.SSL.ValidationCallback
            };

            if (proxy != null)
            {
                handler.Proxy = new Internal.Proxy(proxy);
                handler.UseProxy = true;
            }

            _client = new HttpClient(handler)
            {
                BaseAddress = ApiEndpoint,
                Timeout = Timeout.InfiniteTimeSpan,
            };


            var userAgent = UserAgent;
            if (appIdentifier != null)
            {
                userAgent += " " + appIdentifier;
            }

            _client.DefaultRequestHeaders.Add("User-Agent", userAgent);

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("api:" + key));
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
        }

        public Task<HttpResponseMessage> Request(Method method, string url)
        {
            return Request(method, new Uri(url, UriKind.Relative));
        }

        public Task<HttpResponseMessage> Request(Method method, string url, byte[] body)
        {
            return Request(method, new Uri(url, UriKind.Relative), body);
        }

        public Task<HttpResponseMessage> Request(Method method, string url, Dictionary<string, object> options)
        {
            return Request(method, new Uri(url, UriKind.Relative), options);
        }

        public Task<HttpResponseMessage> Request(Method method, Uri url, byte[] body)
        {
            return Request(method, url, new ByteArrayContent(body));
        }

        public Task<HttpResponseMessage> Request(Method method, Uri url, Dictionary<string, object> options)
        {
            if (method == HttpMethod.Get && options.Count == 0)
            {
                return Request(method, url);
            }

            var json = JsonSerializer.Serialize(options);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            return Request(method, url, body);
        }

        public async Task<HttpResponseMessage> Request(Method method, Uri url, HttpContent body = null)
        {
            for (var retries = RetryCount; retries >= 0; retries--)
            {
                if (retries < RetryCount)
                {
                    await Task.Delay(RetryDelay);
                }

                var request = new HttpRequestMessage(method, url)
                {
                    Content = body
                };

                HttpResponseMessage response;
                try
                {
                    response = await _client.SendAsync(request).ConfigureAwait(false);
                }
                catch (OperationCanceledException err)
                {
                    if (retries > 0) continue;
                    throw new ConnectionException("Timeout while connecting", err);
                }
                catch (Exception err)
                {
                    if (retries > 0) continue;

                    if (err.InnerException != null)
                    {
                        err = err.InnerException;
                    }

                    throw new ConnectionException("Error while connecting: " + err.Message, err);
                }

                if (response.Headers.Contains("Compression-Count"))
                {
                    var compressionCount = response.Headers.GetValues("Compression-Count").First();
                    if (uint.TryParse(compressionCount, out var parsed))
                    {
                        Tinify.CompressionCount = parsed;
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                if (retries > 0 && (uint)response.StatusCode >= 500) continue;

                ErrorData data;
                try
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    data = JsonSerializer.Deserialize<ErrorData>(content) ??
                           new ErrorData() {Message = "Response content was empty.", Error = "ParseError"};
                }
                catch (Exception err)
                {
                    data = new ErrorData
                    {
                        Message = "Error while parsing response: " + err.Message,
                        Error = "ParseError"
                    };
                }
                throw TinifyException.Create(data.Message, data.Error, (uint)response.StatusCode);
            }

            return null;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
