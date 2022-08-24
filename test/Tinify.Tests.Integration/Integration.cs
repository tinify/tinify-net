using MetadataExtractor;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.WebP;
using MetadataExtractor.Formats.Xmp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MetadataDirectory = MetadataExtractor.Directory;
// ReSharper disable InconsistentNaming

namespace TinifyAPI.Tests.Integration
{
    internal sealed class TempFile : IDisposable
    {
        public string Path { get; private set; }

        public TempFile()
        {
            Path = System.IO.Path.GetTempFileName();
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
            try
            {
                if (!string.IsNullOrWhiteSpace(Path)) File.Delete(Path);
            }
            catch
            {
                // ignored
            }

            Path = null;
        }
    }

    internal sealed class ImageMetadata
    {
        private const string ImagePngMimeTypeString = "image/png";
        private const string ImageJpegMimeTypeString = "image/jpeg";
        private const string ImageWebPMimeTypeString = "image/webp";

        private readonly IReadOnlyList<MetadataDirectory> _metaDataDirectories;

        private string _imageFileType;
        public string ImageFileType => _imageFileType ??= GetImageFileType();

        public ImageMetadata(string fileName)
        {
            _metaDataDirectories = ImageMetadataReader.ReadMetadata(fileName);
        }

        public bool IsPng => ImageFileType.Equals(ImagePngMimeTypeString, StringComparison.Ordinal);
        public bool IsJpeg => ImageFileType.Equals(ImageJpegMimeTypeString, StringComparison.Ordinal);
        public bool IsWebP => ImageFileType.Equals(ImageWebPMimeTypeString, StringComparison.Ordinal);

        private string GetImageFileType()
        {
            var fileTypeDir = _metaDataDirectories.OfType<FileTypeDirectory>().FirstOrDefault();
            if (fileTypeDir is null) return "unknown";
            return fileTypeDir.GetObject(FileTypeDirectory.TagDetectedFileMimeType) as string;
        }

        public int GetImageWidth()
        {
            return ImageFileType switch
            {
                ImagePngMimeTypeString => GetWidthFromPng(),
                ImageJpegMimeTypeString => GetWidthFromJpeg(),
                ImageWebPMimeTypeString => GetWidthFromWebP(),
                _ => -1
            };
        }

        public bool ContainsStringInXmpData(string toFind)
        {
            var dir = _metaDataDirectories.FirstOrDefault(d => d.Name.Equals("XMP", StringComparison.Ordinal));
            return (dir is XmpDirectory xmp) &&
                   xmp.GetXmpProperties().Any(pair => pair.Value.Equals(toFind, StringComparison.Ordinal));
        }

        private int GetWidthFromPng()
        {
            var header = _metaDataDirectories.FirstOrDefault(d => d.Name.Equals("PNG-IHDR", StringComparison.Ordinal));
            return (header?.GetObject(PngDirectory.TagImageWidth) as int?) ?? -1;
        }

        private int GetWidthFromJpeg()
        {
            var header = _metaDataDirectories.FirstOrDefault(d => d.Name.Equals("JPEG", StringComparison.Ordinal));
            var tag = header?.GetObject(JpegDirectory.TagImageWidth);
            if (tag is null) return -1;

            // Jpeg exif width tag is a Uint16, so need to use convert
            return Convert.ToInt32(tag);
        }

        private int GetWidthFromWebP()
        {
            var header = _metaDataDirectories.FirstOrDefault(d => d.Name.Equals("WebP", StringComparison.Ordinal));
            return (header?.GetObject(WebPDirectory.TagImageWidth) as int?) ?? -1;
        }
    }

    [TestFixture]
    public class Client_Integration
    {
        private static Task<Source> optimized;

        private const string VoormediaCopyright = "Copyright Voormedia";

        [OneTimeSetUp]
        public static void Init()
        {
            DotNetEnv.Env.Load();

            Tinify.Key = Environment.GetEnvironmentVariable("TINIFY_KEY");
            Tinify.Proxy = Environment.GetEnvironmentVariable("TINIFY_PROXY");

            var unoptimizedPath = Path.Join(AppContext.BaseDirectory, "examples", "voormedia.png");
            optimized = Tinify.FromFile(unoptimizedPath);
        }

        [Test]
        public void Should_Compress_FromFile()
        {
            using (var file = new TempFile())
            {
                optimized.ToFile(file.Path).Wait();

                var size = new FileInfo(file.Path).Length;
                Assert.Greater(size, 1000);
                Assert.Less(size, 1500);

                var metaData = new ImageMetadata(file.Path);
                Assert.That(metaData.IsPng);

                /* width == 137 */
                Assert.AreEqual(137, metaData.GetImageWidth());
                Assert.IsFalse(metaData.ContainsStringInXmpData(VoormediaCopyright));
            }
        }

        [Test]
        public void Should_Compress_FromUrl()
        {
            using (var file = new TempFile())
            {
                var source = Tinify.FromUrl(
                    "https://raw.githubusercontent.com/tinify/tinify-python/master/test/examples/voormedia.png"
                );

                source.ToFile(file.Path).Wait();

                var size = new FileInfo(file.Path).Length;
                Assert.Greater(size, 1000);
                Assert.Less(size, 1500);

                var metaData = new ImageMetadata(file.Path);
                Assert.That(metaData.IsPng);

                /* width == 137 */
                Assert.AreEqual(137, metaData.GetImageWidth());
                Assert.IsFalse(metaData.ContainsStringInXmpData(VoormediaCopyright));
            }
        }

        [Test]
        public void Should_Resize()
        {
            using (var file = new TempFile())
            {
                var options = new { method = "fit", width = 50, height = 20 };
                optimized.Resize(options).ToFile(file.Path).Wait();

                var size = new FileInfo(file.Path).Length;
                Assert.Greater(size, 500);
                Assert.Less(size, 1000);

                var metaData = new ImageMetadata(file.Path);
                Assert.That(metaData.IsPng);

                /* width == 50 */
                Assert.AreEqual(50, metaData.GetImageWidth());
                Assert.IsFalse(metaData.ContainsStringInXmpData(VoormediaCopyright));
            }
        }

        [Test]
        public void Should_PreserveMetadata()
        {
            using (var file = new TempFile())
            {
                var options = new[] {"copyright", "location"};
                optimized.Preserve(options).ToFile(file.Path).Wait();

                var size = new FileInfo(file.Path).Length;
                Assert.Greater(size, 1000);
                Assert.Less(size, 2000);

                var metaData = new ImageMetadata(file.Path);
                Assert.That(metaData.IsPng);

                /* width == 137 */
                Assert.AreEqual(137, metaData.GetImageWidth());
                Assert.IsTrue(metaData.ContainsStringInXmpData(VoormediaCopyright));
            }
        }

        [Test]
        public void Should_Resize_And_PreserveMetadata()
        {
            using var file = new TempFile();
            var resizeOptions = new { method = "fit", width = 50, height = 20 };
            var preserveOptions = new[] { "copyright", "location" };
            optimized.Resize(resizeOptions).Preserve(preserveOptions).ToFile(file.Path).Wait();

            var size = new FileInfo(file.Path).Length;
            Assert.Greater(size, 500);
            Assert.Less(size, 1100);

            var metaData = new ImageMetadata(file.Path);
            Assert.That(metaData.IsPng);

            /* width == 50 */
            Assert.AreEqual(50, metaData.GetImageWidth());
            Assert.IsTrue(metaData.ContainsStringInXmpData(VoormediaCopyright));
        }

        [Test]
        public void Should_Transcode_ToJpeg()
        {
            using var file = new TempFile();
            optimized.Transcode("image/jpeg").Transform(Color.Black).ToFile(file.Path).Wait();

            var metaData = new ImageMetadata(file.Path);
            Assert.That(metaData.IsJpeg);

            /* width == 137 */
            Assert.AreEqual(137, metaData.GetImageWidth());
        }

        [Test]
        public void Should_Transcode_ToWebP()
        {
            using var file = new TempFile();
            optimized.Transcode(new [] {"image/jpeg", "image/webp"}).ToFile(file.Path).Wait();

            var metaData = new ImageMetadata(file.Path);
            Assert.That(metaData.IsWebP);

            /* width == 137 */
            Assert.AreEqual(137, metaData.GetImageWidth());
        }
    }
}
