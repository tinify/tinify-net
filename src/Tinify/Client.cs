﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinifyAPI
{
    using Method = HttpMethod;

    public class Client : IDisposable
    {
        public static readonly Uri ApiEndpoint = new Uri("https://api.tinify.com");

        public static readonly ushort RetryCount = 1;
        public static readonly ushort RetryDelay = 500;

        public static readonly string UserAgent = Internal.Platform.UserAgent;

        HttpClient client;

        public Client(string key, string appIdentifier = null, string proxy = null, bool validateServerCertificate = true)
        {
            var handler = new HttpClientHandler();

            if (validateServerCertificate)
            {
                handler.ServerCertificateCustomValidationCallback = Internal.SSL.ValidationCallback;
            }

            if (proxy != null)
            {
                handler.Proxy = new Internal.Proxy(proxy);
                handler.UseProxy = true;
            }

            client = new HttpClient(handler)
            {
                BaseAddress = ApiEndpoint,
                Timeout = Timeout.InfiniteTimeSpan,
            };


            var userAgent = UserAgent;
            if (appIdentifier != null)
            {
                userAgent = userAgent + " " + appIdentifier;
            }

            client.DefaultRequestHeaders.Add("User-Agent", userAgent);

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("api:" + key));
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
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
            else
            {
                var json = JsonConvert.SerializeObject(options);
                var body = new StringContent(json, Encoding.UTF8, "application/json");
                return Request(method, url, body);
            }
        }

        public async Task<HttpResponseMessage> Request(Method method, Uri url, HttpContent body = null)
        {
            for (short retries = (short) RetryCount; retries >= 0; retries--)
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
                    response = await client.SendAsync(request).ConfigureAwait(false);
                }
                catch (OperationCanceledException err)
                {
                    if (retries > 0) continue;
                    throw new ConnectionException("Timeout while connecting", err);
                }
                catch (System.Exception err)
                {
                    if (err.InnerException != null)
                    {
                        err = err.InnerException;
                    }

                    if (retries > 0) continue;
                    throw new ConnectionException("Error while connecting: " + err.Message, err);
                }

                if (response.Headers.Contains("Compression-Count"))
                {
                    var compressionCount = response.Headers.GetValues("Compression-Count").First();
                    uint parsed;
                    if (uint.TryParse(compressionCount, out parsed))
                    {
                        Tinify.CompressionCount = parsed;
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                var data = new { message = "", error = "" };
                try
                {
                    data = JsonConvert.DeserializeAnonymousType(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false),
                        data
                    );
                }
                catch (System.Exception err)
                {
                    data = new {
                        message = "Error while parsing response: " + err.Message,
                        error = "ParseError"
                    };
                }

                if (retries > 0 && (uint) response.StatusCode >= 500) continue;
                throw TinifyException.Create(data.message, data.error, (uint) response.StatusCode);
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }
            }
        }
    }
}
