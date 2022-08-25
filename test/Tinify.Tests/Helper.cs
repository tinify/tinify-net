using NUnit.Framework;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace TinifyAPI.Tests
{
    internal static class Helper
    {
        private static readonly FieldInfo HttpClientField = typeof(Client)
            .GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic);

#if NET472 || NET48
        private static readonly FieldInfo HttpHandlerField = typeof(HttpMessageInvoker)
            .GetField("handler", BindingFlags.Instance | BindingFlags.NonPublic);
#else
        private static readonly FieldInfo HttpHandlerField = typeof(HttpMessageInvoker)
            .GetField("_handler", BindingFlags.Instance | BindingFlags.NonPublic);
#endif

        public static MockHttpMessageHandler MockHandler;
        public static HttpRequestMessage LastRequest;
        public static string LastBody;

        public static void MockClient(Client test)
        {
            MockHandler = new MockHttpMessageHandler();

            /* Terrible hack to get/mock/replace client property. */
            var client = (HttpClient) HttpClientField.GetValue(test);
            HttpHandlerField.SetValue(client, MockHandler);

            Client.RetryDelay = 10;
        }

        public static void EnqueueShrink(Client test)
        {
            MockClient(test);

            MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                LastRequest = req;
                if (req.Content != null)
                {
                    LastBody = req.Content.ReadAsStringAsync().Result;
                }

                var res = new HttpResponseMessage(HttpStatusCode.Created);
                res.Headers.Add("Location", "https://api.tinify.com/foo.png");
                res.Headers.Add("Compression-Count", "12");
                return res;
            });
        }

        public static void EnqueueShrinkAndResult(Client test, string body)
        {
            MockClient(test);

            MockHandler.Expect("https://api.tinify.com/shrink").Respond(_ =>
            {
                var res = new HttpResponseMessage(HttpStatusCode.Created);
                res.Headers.Add("Location", "https://api.tinify.com/some/location");
                res.Headers.Add("Compression-Count", "12");
                return res;
            });

            MockHandler.Expect("https://api.tinify.com/some/location").Respond(req =>
            {
                LastRequest = req;
                if (req.Content != null)
                {
                    LastBody = req.Content.ReadAsStringAsync().Result;
                }

                var res = new HttpResponseMessage(HttpStatusCode.OK);
                res.Content = new StringContent(body);
                return res;
            });
        }

        public static void EnqueuShrinkAndStore(Client test)
        {
            MockClient(test);

            MockHandler.Expect("https://api.tinify.com/shrink").Respond(_ =>
            {
                var res = new HttpResponseMessage(HttpStatusCode.Created);
                res.Headers.Add("Location", "https://api.tinify.com/some/location");
                res.Headers.Add("Compression-Count", "12");
                return res;
            });

            MockHandler.Expect("https://api.tinify.com/some/location").Respond(req =>
            {
                LastRequest = req;
                if (req.Content != null)
                {
                    LastBody = req.Content.ReadAsStringAsync().Result;
                }

                var res = new HttpResponseMessage(HttpStatusCode.OK);
                res.Headers.Add("Location", "https://bucket.s3.amazonaws.com/example");
                return res;
            });
        }

        // Helper method added due to a behavior change in .Net 6.0 where instead of returning null,
        // HttpContent will be of type EmptyContentType
#if NET5_0_OR_GREATER
        private static readonly System.Type EmptyContentType = typeof(HttpContent).Assembly.GetType("System.Net.Http.EmptyContent");

        public static void AssertEmptyResponseContent(HttpContent content) => Assert.IsInstanceOf(EmptyContentType, content);
#else
        public static void AssertEmptyResponseContent(HttpContent content) => Assert.AreEqual(null, content);
#endif
    }
}
