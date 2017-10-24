using NUnit.Framework;
using System.Net.Http;
using System.Text;

namespace TinifyAPI.Tests
{
    [TestFixture]
    public class Result_With_MetaAndData
    {
        Result subject;

        [SetUp]
        public void SetUp()
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent("image data")
            };

            response.Headers.Add("Image-Width", "100");
            response.Headers.Add("Image-Height", "60");
            response.Content.Headers.Clear();
            response.Content.Headers.Add("Content-Length", "450");
            response.Content.Headers.Add("Content-Type", "image/png");
            subject = new Result(response.Headers, response.Content.Headers, Encoding.ASCII.GetBytes("image data"));
        }

        [Test]
        public void Width_Should_ReturnImageWidth()
        {
            Assert.AreEqual(100, subject.Width);
        }

        [Test]
        public void Height_Should_ReturnImageHeight()
        {
            Assert.AreEqual(60, subject.Height);
        }

        [Test]
        public void Size_Should_ReturnContentLength()
        {
            Assert.AreEqual(450, subject.Size);
        }

        [Test]
        public void ContentType_Should_ReturnMimeType()
        {
            Assert.AreEqual("image/png", subject.ContentType);
        }

        [Test]
        public void ToBuffer_Should_ReturnImageData()
        {
            Assert.AreEqual(Encoding.ASCII.GetBytes("image data"), subject.ToBuffer());
        }
    }

    [TestFixture]
    public class Result_Without_MetaAndData
    {
        Result subject;

        [SetUp]
        public void SetUp()
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent("")
            };

            response.Content.Headers.Clear();
            response.Content.Headers.ContentLength = null;
            subject = new Result(response.Headers, response.Content.Headers, null);
        }

        [Test]
        public void Width_Should_ReturnNull()
        {
            Assert.AreEqual(null, subject.Width);
        }

        [Test]
        public void Height_Should_ReturnNull()
        {
            Assert.AreEqual(null, subject.Height);
        }

        [Test]
        public void Location_Should_ReturnImageNull()
        {
            Assert.AreEqual(null, subject.Location);
        }

        [Test]
        public void Size_Should_ReturnNull()
        {
            Assert.AreEqual(null, subject.Size);
        }

        [Test]
        public void ContentType_Should_ReturnNull()
        {
            Assert.AreEqual(null, subject.ContentType);
        }

        [Test]
        public void ToBuffer_Should_ReturnNull()
        {
            Assert.AreEqual(null, subject.ToBuffer());
        }
    }
}
