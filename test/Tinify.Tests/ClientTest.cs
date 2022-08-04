using NUnit.Framework;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TinifyAPI.Tests
{
    [TestFixture]
    public class Client_Request_WhenValid
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.EnqueuShrink(Subject);
        }

        [TearDown]
        public void TearDown()
        {
            Helper.MockHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void Should_IssueRequest()
        {
            Subject.Request(HttpMethod.Get, "/shrink").Wait();
            Assert.AreEqual(
                "Basic YXBpOmtleQ==",
                Helper.LastRequest.Headers.GetValues("Authorization").FirstOrDefault()
            );
        }

        [Test]
        public void Should_IssueRequest_ToEndpoint()
        {
            Subject.Request(HttpMethod.Get, "/shrink").Wait();
            Assert.AreEqual("https://api.tinify.com/shrink", Helper.LastRequest.RequestUri.ToString());
        }

        [Test]
        public void Should_IssueRequest_WithMethod()
        {
            Subject.Request(HttpMethod.Post, "/shrink").Wait();
            Assert.AreEqual("POST", Helper.LastRequest.Method.ToString());
        }

        [Test]
        public void Should_ReturnResponse()
        {
            var response = Subject.Request(HttpMethod.Post, "/shrink").Result;
            Assert.AreEqual(
                "https://api.tinify.com/foo.png",
                response.Headers.GetValues("Location").FirstOrDefault()
            );
        }

        [Test]
        public void Should_IssueRequest_WithoutBody_WhenOptionsAreEmpty()
        {
            var response = Subject.Request(HttpMethod.Post, "/shrink").Result;
            Assert.IsInstanceOf(Helper.EmptyContentType, response.Content);
        }

        [Test]
        public void Should_IssueRequest_WithoutContentType_WhenOptionsAreEmpty()
        {
            Subject.Request(HttpMethod.Post, "/shrink").Wait();
            /* Content is null so none of Content.Headers can be set. */
            Assert.AreEqual(null, Helper.LastRequest.Content);
        }

        [Test]
        public void Should_IssueRequest_WithJsonBody()
        {
            var opts = new Dictionary<string, object>();
            opts.Add("hello", "world");
            Subject.Request(HttpMethod.Post, "/shrink", opts).Wait();
            Assert.AreEqual("{\"hello\":\"world\"}", Helper.LastBody);
        }

        [Test]
        public void Should_IssueRequest_WithUserAgent()
        {
            Subject.Request(HttpMethod.Post, "/shrink").Wait();
            Assert.AreEqual(
                Internal.Platform.UserAgent,
                string.Join(" ", Helper.LastRequest.Headers.GetValues("User-Agent"))
            );
        }

        [Test]
        public void Should_UpdateCompressionCount()
        {
            Subject.Request(HttpMethod.Post, "/shrink").Wait();
            Assert.AreEqual(12, Tinify.CompressionCount);
        }
    }

    [TestFixture]
    public class Client_Request_WhenValid_WithAppId
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key, "TestApp/0.1");
            Helper.EnqueuShrink(Subject);
        }

        [TearDown]
        public void TearDown()
        {
            Helper.MockHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        public void Should_IssueRequest_WithUserAgent()
        {
            Subject.Request(HttpMethod.Post, "/shrink").Wait();
            Assert.AreEqual(
                Internal.Platform.UserAgent + " TestApp/0.1",
                string.Join(" ", Helper.LastRequest.Headers.GetValues("User-Agent"))
            );
        }
    }

    [TestFixture]
    public class Client_Request_WhenValid_WithProxy
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key, null, "http://user:pass@localhost:8080");
            Helper.EnqueuShrink(Subject);
        }

        [TearDown]
        public void TearDown()
        {
            Helper.MockHandler.VerifyNoOutstandingExpectation();
        }

        [Test]
        [Ignore("test proxy")]
        public void Should_IssueRequest_WithProxyAuthorization()
        {
            Subject.Request(HttpMethod.Post, "/shrink").Wait();
            Assert.AreEqual(
                "Basic dXNlcjpwYXNz",
                string.Join(" ", Helper.LastRequest.Headers.GetValues("Proxy-Authorization"))
            );
        }
    }

    [TestFixture]
    public class Client_Request_WithTimeout_Once
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new TaskCanceledException();
            });

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 201
            );
        }

        [Test]
        public void Should_ReturnResponse()
        {
            var response = Subject.Request(HttpMethod.Post, "/shrink").Result;
            Assert.IsInstanceOf(Helper.EmptyContentType, response.Content);
        }
    }

    [TestFixture]
    public class Client_Request_WithTimeout_Repeatedly
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new TaskCanceledException();
            });

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new TaskCanceledException();
            });
        }

        [Test]
        public void Should_ThrowConnectionException()
        {
            var error = Assert.ThrowsAsync<ConnectionException>(async () =>
            {
                await Subject.Request(HttpMethod.Post, "/shrink");
            });

            Assert.AreEqual(
                "Timeout while connecting",
                error.Message
            );
        }
    }

    [TestFixture]
    public class Client_Request_WithSocketError_Once
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new HttpRequestException("An error occurred while sending the request");
            });

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 201
            );
        }

        [Test]
        public void Should_ReturnResponse()
        {
            var response = Subject.Request(HttpMethod.Post, "/shrink").Result;
            Assert.IsInstanceOf(Helper.EmptyContentType, response.Content);
        }
    }

    [TestFixture]
    public class Client_Request_WithSocketError_Repeatedly
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new HttpRequestException("An error occurred while sending the request");
            });

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new HttpRequestException("An error occurred while sending the request");
            });
        }

        [Test]
        public void Should_ThrowConnectionException()
        {
            var error = Assert.ThrowsAsync<ConnectionException>(async () =>
            {
                await Subject.Request(HttpMethod.Post, "/shrink");
            });

            Assert.AreEqual(
                "Error while connecting: An error occurred while sending the request",
                error.Message
            );
        }
    }

    [TestFixture]
    public class Client_Request_WithUnexpectedError_Once
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new System.Exception("some error");
            });

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 201
            );
        }

        [Test]
        public void Should_ReturnResponse()
        {
            var response = Subject.Request(HttpMethod.Post, "/shrink").Result;
            Assert.IsInstanceOf(Helper.EmptyContentType, response.Content);
        }
    }

    [TestFixture]
    public class Client_Request_WithUnexpectedError_Repeatedly
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new System.Exception("some error");
            });

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                throw new System.Exception("some error");
            });
        }

        [Test]
        public void Should_ThrowConnectionException()
        {
            var error = Assert.ThrowsAsync<ConnectionException>(async () =>
            {
                await Subject.Request(HttpMethod.Post, "/shrink");
            });

            Assert.AreEqual(
                "Error while connecting: some error",
                error.Message
            );
        }
    }

    [TestFixture]
    public class Client_Request_WithServerError_Once
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 584,
                new StringContent("{\"error\":\"InternalServerError\",\"message\":\"Oops!\"}")
            );

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 201
            );
        }

        [Test]
        public void Should_ReturnResponse()
        {
            var response = Subject.Request(HttpMethod.Post, "/shrink").Result;
            Assert.IsInstanceOf(Helper.EmptyContentType, response.Content);
        }
    }

    [TestFixture]
    public class Client_Request_WithServerError_Repeatedly
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 584,
                new StringContent("{\"error\":\"InternalServerError\",\"message\":\"Oops!\"}")
            );

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 584,
                new StringContent("{\"error\":\"InternalServerError\",\"message\":\"Oops!\"}")
            );
        }

        [Test]
        public void Should_ThrowConnectionException()
        {
            var error = Assert.ThrowsAsync<ServerException>(async () =>
            {
                await Subject.Request(HttpMethod.Post, "/shrink");
            });

            Assert.AreEqual(
                "Oops! (HTTP 584/InternalServerError)",
                error.Message
            );
        }
    }

    [TestFixture]
    public class Client_Request_WithBadServerResponse_Once
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 543,
                new StringContent("<!-- this is not json -->")
            );

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 201
            );
        }

        [Test]
        public void Should_ReturnResponse()
        {
            var response = Subject.Request(HttpMethod.Post, "/shrink").Result;
            Assert.IsInstanceOf(Helper.EmptyContentType, response.Content);
        }
    }

    [TestFixture]
    public class Client_Request_WithBadServerResponse_Repeatedly
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 543,
                new StringContent("<!-- this is not json -->")
            );

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 543,
                new StringContent("<!-- this is not json -->")
            );
        }

        [Test]
        public void Should_ThrowConnectionException()
        {
            var error = Assert.ThrowsAsync<ServerException>(async () =>
            {
                await Subject.Request(HttpMethod.Post, "/shrink");
            });

            Assert.AreEqual(
                "Error while parsing response: Unexpected character encountered while " +
                "parsing value: <. Path '', line 0, position 0. (HTTP 543/ParseError)",
                error.Message
            );
        }
    }

    [TestFixture]
    public class Client_Request_WithClientError
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 492,
                new StringContent("{\"error\":\"BadRequest\",\"message\":\"Oops!\"}")
            );
        }

        [Test]
        public void Should_ThrowClientException()
        {
            var error = Assert.ThrowsAsync<ClientException>(async () =>
            {
                await Subject.Request(HttpMethod.Post, "/shrink");
            });

            Assert.AreEqual(
                "Oops! (HTTP 492/BadRequest)",
                error.Message
            );
        }
    }

    [TestFixture]
    public class Client_Request_WithBadCredentials
    {
        public Client Subject { get; set; }
        string key = "key";

        [SetUp]
        public void SetUp()
        {
            Subject = new Client(key);
            Helper.MockClient(Subject);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                HttpStatusCode.Unauthorized,
                new StringContent("{\"error\":\"Unauthorized\",\"message\":\"Oops!\"}")
            );
        }

        [Test]
        public void Should_ThrowAccountException()
        {
            var error = Assert.ThrowsAsync<AccountException>(async () =>
            {
                await Subject.Request(HttpMethod.Post, "/shrink");
            });

            Assert.AreEqual(
                "Oops! (HTTP 401/Unauthorized)",
                error.Message
            );
        }
    }
}
