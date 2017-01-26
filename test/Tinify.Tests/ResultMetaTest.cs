using NUnit.Framework;

using System;
using System.Net.Http;

namespace TinifyAPI.Tests
{
    [TestFixture]
    public class ResultMeta_With_Meta
    {
        ResultMeta subject;

        [SetUp]
        public void SetUp()
        {
            var response = new HttpResponseMessage();
            var headers = response.Headers;
            headers.Add("Image-Width", "100");
            headers.Add("Image-Height", "60");
            headers.Add("Location", "https://example.com/image.png");
            subject = new ResultMeta(headers);
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
        public void Location_Should_ReturnImageLocation()
        {
            Assert.AreEqual(new Uri("https://example.com/image.png"), subject.Location);
        }
    }

    [TestFixture]
    public class ResultMeta_Without_Meta
    {
        ResultMeta subject;

        [SetUp]
        public void SetUp()
        {
            var response = new HttpResponseMessage();
            subject = new ResultMeta(response.Headers);
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
    }
}
