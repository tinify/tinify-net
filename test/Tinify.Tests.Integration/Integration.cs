using NUnit.Framework;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

// using TinifyClient;
// using TinifyAPI;
// using TinifyNET;
// using Tinify.Net;
//
// class Compress
// {
//   static void Main()
//   {
//     Tinify.Key = "YOUR_API_KEY";
//     Tinify.FromFile("unoptimized.png").ToFile("optimized.png");
//   }
// }

namespace TinifyAPI.Tests.Integration
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
    public class Client_Integration
    {
        static Task<Source> optimized;

        [OneTimeSetUp]
        public static void Init()
        {
            Tinify.Key = Environment.GetEnvironmentVariable("TINIFY_KEY");
            Tinify.Proxy = Environment.GetEnvironmentVariable("TINIFY_PROXY");

            var unoptimizedPath = AppContext.BaseDirectory + "/examples/voormedia.png";
            optimized = Tinify.FromFile(unoptimizedPath);
        }

        [Test]
        public void Should_Compress_FromFile()
        {
            using (var file = new TempFile())
            {
                optimized.ToFile(file.Path).Wait();

                var size = new FileInfo(file.Path).Length;
                var contents = File.ReadAllBytes(file.Path);

                Assert.Greater(size, 1000);
                Assert.Less(size, 1500);

                /* width == 137 */
                CollectionAssert.IsSubsetOf(new byte[] {0, 0, 0, 0x89}, contents);
                CollectionAssert.IsNotSubsetOf(
                    Encoding.ASCII.GetBytes("Copyright Voormedia"),
                    contents
                );
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
                var contents = File.ReadAllBytes(file.Path);

                Assert.Greater(size, 1000);
                Assert.Less(size, 1500);

                /* width == 137 */
                CollectionAssert.IsSubsetOf(new byte[] {0, 0, 0, 0x89}, contents);
                CollectionAssert.IsNotSubsetOf(
                    Encoding.ASCII.GetBytes("Copyright Voormedia"),
                    contents
                );
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
                var contents = File.ReadAllBytes(file.Path);

                Assert.Greater(size, 500);
                Assert.Less(size, 1000);

                /* width == 50 */
                CollectionAssert.IsSubsetOf(new byte[] {0, 0, 0, 0x32}, contents);
                CollectionAssert.IsNotSubsetOf(
                    Encoding.ASCII.GetBytes("Copyright Voormedia"),
                    contents
                );
            }
        }

        [Test]
        public void Should_PreserveMetadata()
        {
            using (var file = new TempFile())
            {
                var options = new string[] {"copyright", "location"};
                optimized.Preserve(options).ToFile(file.Path).Wait();

                var size = new FileInfo(file.Path).Length;
                var contents = File.ReadAllBytes(file.Path);

                Assert.Greater(size, 1000);
                Assert.Less(size, 2000);

                /* width == 137 */
                CollectionAssert.IsSubsetOf(new byte[] {0, 0, 0, 0x89}, contents);
                CollectionAssert.IsSubsetOf(
                    Encoding.ASCII.GetBytes("Copyright Voormedia"),
                    contents
                );
            }
        }
    }
}
