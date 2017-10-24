using NUnit.Framework;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;

namespace TinifyAPI.Tests
{
    public abstract class Reset
    {
        [TearDown]
        public void TearDown()
        {
            Tinify.Key = null;
            Tinify.Proxy = null;
        }
    }

    [TestFixture]
    public class Tinify_Key : Reset
    {
        [Test]
        public void Should_ResetClient_WithNewKey()
        {
            Tinify.Key = "abcde";
            var client = Tinify.Client;
            Tinify.Key = "fghij";

            Helper.EnqueuShrink(Tinify.Client);
            Tinify.Client.Request(HttpMethod.Get, "/shrink").Wait();

            Assert.AreEqual(
                "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("api:fghij")),
                Helper.LastRequest.Headers.Authorization.ToString()
            );
        }
    }

    [TestFixture]
    public class Tinify_AppIdentifier : Reset
    {
        [Test]
        public void Should_ResetClient_WithNewAppIdentifier()
        {
            Tinify.Key = "abcde";
            Tinify.AppIdentifier = "MyApp/1.0";
            var client = Tinify.Client;
            Tinify.AppIdentifier = "MyApp/2.0";

            Helper.EnqueuShrink(Tinify.Client);
            Tinify.Client.Request(HttpMethod.Get, "/shrink").Wait();

            Assert.AreEqual(
                Client.UserAgent + " MyApp/2.0",
                Helper.LastRequest.Headers.UserAgent.ToString()
            );
        }
    }

    [TestFixture]
    public class Tinify_Proxy : Reset
    {
        [Test]
        [Ignore("test proxy")]
        public void Should_ResetClient_WithNewProxy()
        {
            Tinify.Key = "abcde";
            Tinify.Proxy = "http://localhost";
            var client = Tinify.Client;
            Tinify.Proxy = "http://user:pass@localhost:8080";

            Helper.EnqueuShrink(Tinify.Client);
            Tinify.Client.Request(HttpMethod.Get, "/shrink").Wait();

            Assert.AreEqual(
                "Basic dXNlcjpwYXNz",
                string.Join(" ", Helper.LastRequest.Headers.GetValues("Proxy-Authorization"))
            );
        }
    }

    [TestFixture]
    public class Tinify_Client : Reset
    {
        [Test]
        public void WithKey_Should_ReturnClient()
        {
            Tinify.Key = "abcde";
            Assert.IsInstanceOf<Client>(Tinify.Client);
        }

        [Test]
        public void WithoutKey_Should_ThrowException()
        {
            var error = Assert.Throws<AccountException>(() =>
            {
                var client = Tinify.Client;
            });

            Assert.AreEqual(
                "Provide an API key with Tinify.Key = ...",
                error.Message
            );
        }

        [Test]
        public void WithInvalidProxy_Should_ThrowException()
        {
            Tinify.Key = "abcde";
            Tinify.Proxy = "http-bad-url";
            var error = Assert.Throws<ConnectionException>(() =>
            {
                var client = Tinify.Client;
            });

            Assert.AreEqual(
                "Invalid proxy: cannot parse 'http-bad-url'",
                error.Message
            );
        }
    }

    [TestFixture]
    public class Tinify_Validate : Reset
    {
        [Test]
        public void WithValidKey_Should_ReturnTrue()
        {
            Tinify.Key = "valid";

            Helper.MockClient(Tinify.Client);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                HttpStatusCode.BadRequest,
                new StringContent("{\"error\":\"Input missing\",\"message\":\"No input\"}")
            );

            Assert.AreEqual(true, Tinify.Validate().Result);
        }

        [Test]
        public void WithLimitedKey_Should_ReturnTrue()
        {
            Tinify.Key = "valid";

            Helper.MockClient(Tinify.Client);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                (HttpStatusCode) 429,
                new StringContent("{\"error\":\"Too may requests\",\"message\":\"Your monthly limit has been exceeded\"}")
            );

            Assert.AreEqual(true, Tinify.Validate().Result);
        }

        [Test]
        public void WithError_Should_ThrowException()
        {
            Tinify.Key = "valid";

            Helper.MockClient(Tinify.Client);
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                HttpStatusCode.Unauthorized,
                new StringContent("{\"error\":\"Unauthorized\",\"message\":\"Credentials are invalid\"}")
            );

            Assert.ThrowsAsync<AccountException>(async () =>
            {
                await Tinify.Validate();
            });
        }
    }

    [TestFixture]
    public class Tinify_FromBuffer : Reset
    {
        [SetUp]
        public void SetUp()
        {
            Tinify.Key = "valid";
            Helper.MockClient(Tinify.Client);

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                var res = new HttpResponseMessage(HttpStatusCode.Created);
                res.Headers.Add("Location", "https://api.tinify.com/some/location");
                return res;
            });
        }

        [Test]
        public void Should_ReturnSourceTask()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.IsInstanceOf<Task<Source>>(Source.FromBuffer(buffer));
        }
    }

    [TestFixture]
    public class Tinify_FromFile : Reset
    {
        [SetUp]
        public void SetUp()
        {
            Tinify.Key = "valid";
            Helper.MockClient(Tinify.Client);

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                var res = new HttpResponseMessage(HttpStatusCode.Created);
                res.Headers.Add("Location", "https://api.tinify.com/some/location");
                return res;
            });
        }

        [Test]
        public void Should_ReturnSourceTask()
        {
            Assert.IsInstanceOf<Task<Source>>(
                Tinify.FromFile(AppContext.BaseDirectory + "/examples/dummy.png")
            );
        }
    }

    [TestFixture]
    public class Tinify_FromUrl : Reset
    {
        [SetUp]
        public void SetUp()
        {
            Tinify.Key = "valid";
            Helper.MockClient(Tinify.Client);

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(req =>
            {
                var res = new HttpResponseMessage(HttpStatusCode.Created);
                res.Headers.Add("Location", "https://api.tinify.com/some/location");
                return res;
            });
        }

        [Test]
        public void Should_ReturnSourceTask()
        {
            Assert.IsInstanceOf<Task<Source>>(
                Source.FromUrl("http://example.com/test.jpg")
            );
        }
    }
}
