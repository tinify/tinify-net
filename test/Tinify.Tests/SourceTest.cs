using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;

namespace TinifyAPI.Tests
{
    sealed class TempFile : IDisposable
    {
        private string path;

        public string Path
        {
            get { return path; }
        }

        public TempFile()
        {
            path = System.IO.Path.GetTempFileName();
        }

        ~TempFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) GC.SuppressFinalize(this);
            try { File.Delete(path); } catch { }
            path = null;
        }
    }

    [TestFixture]
    public class Source_WithInvalidApiKey
    {
        [SetUp]
        public void SetUp()
        {
            Tinify.Key = "invalid";
            Helper.MockClient(Tinify.Client);

            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                HttpStatusCode.Unauthorized,
                new StringContent("{'error':'Unauthorized','message':'Credentials are invalid'}")
            );
        }

        [Test]
        public void FromFile_Should_ThrowAccountException()
        {
            Assert.ThrowsAsync<AccountException>(async () =>
            {
                await Source.FromFile("test/Tinify.Tests/examples/dummy.png");
            });
        }

        [Test]
        public void FromBuffer_Should_ThrowAccountException()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.ThrowsAsync<AccountException>(async () =>
            {
                await Source.FromBuffer(buffer);
            });
        }

        [Test]
        public void FromUrl_Should_ThrowAccountException()
        {
            Assert.ThrowsAsync<AccountException>(async () =>
            {
                await Source.FromUrl("http://example.com/test.jpg");
            });
        }
    }

    [TestFixture]
    public class Source_WithValidApiKey
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

            Helper.MockHandler.Expect("https://api.tinify.com/some/location").Respond(
                HttpStatusCode.OK,
                new StringContent("compressed file")
            );
        }

        [Test]
        public void FromFile_Should_ReturnSourceTask()
        {
            Assert.IsInstanceOf<Task<Source>>(
                Source.FromFile("test/Tinify.Tests/examples/dummy.png")
            );
        }

        [Test]
        public void FromFile_Should_ReturnSourceTask_WithData()
        {
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("compressed file"),
                Source.FromFile("test/Tinify.Tests/examples/dummy.png").ToBuffer().Result
            );
        }

        [Test]
        public void FromBuffer_Should_ReturnSourceTask()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.IsInstanceOf<Task<Source>>(Source.FromBuffer(buffer));
        }

        [Test]
        public void FromBuffer_Should_ReturnSourceTask_WithData()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("compressed file"),
                Source.FromBuffer(buffer).ToBuffer().Result
            );
        }

        [Test]
        public void FromUrl_Should_ReturnSourceTask()
        {
            Assert.IsInstanceOf<Task<Source>>(
                Source.FromUrl("http://example.com/test.jpg")
            );
        }

        [Test]
        public void FromUrl_Should_ReturnSourceTask_WithData()
        {
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("compressed file"),
                Source.FromUrl("http://example.com/test.jpg").ToBuffer().Result
            );
        }

        [Test]
        public void FromUrl_Should_ThrowException_IfRequestIsNotOk()
        {
            Helper.MockHandler.ResetExpectations();
            Helper.MockHandler.Expect("https://api.tinify.com/shrink").Respond(
                HttpStatusCode.BadRequest,
                new StringContent("{'error':'Source not found','message':'Cannot parse URL'}")
            );

            Assert.ThrowsAsync<ClientException>(async () =>
            {
                await Source.FromUrl("file://wrong");
            });
        }

        [Test]
        public void GetResult_Should_ReturnResultTask()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.IsInstanceOf<Task<Result>>(
                Source.FromBuffer(buffer).GetResult()
            );
        }

        [Test]
        public void Preserve_Should_ReturnSourceTask()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.IsInstanceOf<Task<Source>>(
                Source.FromBuffer(buffer).Preserve("copyright", "location")
            );
        }

        [Test]
        public void Preserve_Should_ReturnSourceTask_WithData()
        {
            Helper.EnqueuShrinkAndResult(Tinify.Client, "copyrighted file");

            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("copyrighted file"),
                Source.FromBuffer(buffer).Preserve("copyright", "location").ToBuffer().Result
            );

            Assert.AreEqual(
                "{\"preserve\":[\"copyright\",\"location\"]}",
                Helper.LastBody
            );
        }

        [Test]
        public void Preserve_Should_ReturnSourceTask_WithData_ForArray()
        {
            Helper.EnqueuShrinkAndResult(Tinify.Client, "copyrighted file");

            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("copyrighted file"),
                Source.FromBuffer(buffer).Preserve(new string[] {"copyright", "location"}).ToBuffer().Result
            );

            Assert.AreEqual(
                "{\"preserve\":[\"copyright\",\"location\"]}",
                Helper.LastBody
            );
        }

        [Test]
        public void Preserve_Should_IncludeOtherOptions_IfSet()
        {
            Helper.EnqueuShrinkAndResult(Tinify.Client, "copyrighted resized file");

            var resizeOptions = new { width = 100, height = 60 };
            var preserveOptions = new string[] {"copyright", "location"};

            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("copyrighted resized file"),
                Source.FromBuffer(buffer).Resize(resizeOptions).Preserve(preserveOptions).ToBuffer().Result
            );

            Assert.AreEqual(
                "{\"resize\":{\"width\":100,\"height\":60},\"preserve\":[\"copyright\",\"location\"]}",
                Helper.LastBody
            );
        }

        [Test]
        public void Resize_Should_ReturnSourceTask()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.IsInstanceOf<Task<Source>>(
                Source.FromBuffer(buffer).Resize(new { width = 400 })
            );
        }

        [Test]
        public void Resize_Should_ReturnSourceTask_WithData()
        {
            Helper.EnqueuShrinkAndResult(Tinify.Client, "small file");

            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("small file"),
                Source.FromBuffer(buffer).Resize(new { width = 400 }).ToBuffer().Result
            );

            Assert.AreEqual(
                "{\"resize\":{\"width\":400}}",
                Helper.LastBody
            );
        }

        [Test]
        public void Store_Should_ReturnResultMetaTask()
        {
            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.IsInstanceOf<Task<ResultMeta>>(
                Source.FromBuffer(buffer).Store(new { service = "s3" })
            );
        }

        [Test]
        public void Store_Should_ReturnResultMetaTask_WithLocation()
        {
            Helper.EnqueuShrinkAndStore(Tinify.Client);

            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                new Uri("https://bucket.s3.amazonaws.com/example"),
                Source.FromBuffer(buffer).Store(new { service = "s3" }).Result.Location
            );

            Assert.AreEqual(
                "{\"store\":{\"service\":\"s3\"}}",
                Helper.LastBody
            );
        }

        [Test]
        public void Store_Should_IncludeOtherOptions_IfSet()
        {
            Helper.EnqueuShrinkAndStore(Tinify.Client);

            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                new Uri("https://bucket.s3.amazonaws.com/example"),
                Source.FromBuffer(buffer).Resize(new { width = 400 }).Store(new { service = "s3" }).Result.Location
            );

            Assert.AreEqual(
                "{\"resize\":{\"width\":400},\"store\":{\"service\":\"s3\"}}",
                Helper.LastBody
            );
        }

        [Test]
        public void ToBuffer_Should_ReturnImageData()
        {
            Helper.EnqueuShrinkAndResult(Tinify.Client, "compressed file");

            var buffer = Encoding.ASCII.GetBytes("png file");
            Assert.AreEqual(
                Encoding.ASCII.GetBytes("compressed file"),
                Source.FromBuffer(buffer).ToBuffer().Result
            );
        }

        [Test]
        public void ToFile_Should_StoreImageData()
        {
            Helper.EnqueuShrinkAndResult(Tinify.Client, "compressed file");

            var buffer = Encoding.ASCII.GetBytes("png file");
            using (var file = new TempFile())
            {
                Source.FromBuffer(buffer).ToFile(file.Path).Wait();
                Assert.AreEqual(
                    Encoding.ASCII.GetBytes("compressed file"),
                    File.ReadAllBytes(file.Path)
                );
            }
        }
    }
}
