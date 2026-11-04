using NUnit.Framework;

using System.Text;
// ReSharper disable InconsistentNaming

namespace TinifyAPI.Tests
{
    [TestFixture]
    public class FormatTest
    {
        [SetUp]
        public void SetUp()
        {
            Tinify.Key = "valid";
        }

        [TearDown]
        public void TearDown()
        {
            Tinify.Key = null;
        }

        [Test]
        public void Format_Should_ExposeSupportedMediaTypes()
        {
            Assert.AreEqual("image/webp", Format.WebP);
            Assert.AreEqual("image/png", Format.Png);
            Assert.AreEqual("image/jpeg", Format.Jpeg);
            Assert.AreEqual("image/jpg", Format.Jpg);
            Assert.AreEqual("image/avif", Format.Avif);
            Assert.AreEqual("image/jxl", Format.Jxl);
            Assert.AreEqual("*/*", Format.Any);
        }

        [Test]
        public void Convert_WithFormat_Should_SerializeMediaType()
        {
            Helper.EnqueueShrinkAndResult(Tinify.Client, "transcoded file");
            var buffer = Encoding.ASCII.GetBytes("png file");

            Source.FromBuffer(buffer).Convert(new {type = Format.Jxl}).ToBuffer().Wait();

            Assert.AreEqual(
                "{\"convert\":{\"type\":\"image/jxl\"}}",
                Helper.LastBody);
        }

        [Test]
        public void Convert_WithMultipleFormats_Should_SerializeMediaTypes()
        {
            Helper.EnqueueShrinkAndResult(Tinify.Client, "transcoded file");
            var buffer = Encoding.ASCII.GetBytes("png file");

            Source.FromBuffer(buffer)
                .Convert(new {type = new[] {Format.Jxl, Format.WebP}})
                .ToBuffer().Wait();

            Assert.AreEqual(
                "{\"convert\":{\"type\":[\"image/jxl\",\"image/webp\"]}}",
                Helper.LastBody);
        }
    }
}
