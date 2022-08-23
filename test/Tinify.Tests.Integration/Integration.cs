using NUnit.Framework;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

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

    internal static class Helper
    {
        public static bool ContainsSubsequence(this byte[] source, byte[] subsequence)
        {
            if (source == subsequence) return true;
            if (source.Length < subsequence.Length) return false;
            return Search(source, subsequence) != -1;
        }

        /*!
         * Code snippet for Search function from StackExchange Network question on <https://stackoverflow.com/questions/283456/> 
         * Answered and copied from <https://stackoverflow.com/a/38625726> 
         * Licensed under CC BY-SA 3.0 <http://creativecommons.org/licenses/by-sa/3.0/>
         */
        private static int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;
        
                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--) 
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }
    }

    [TestFixture]
    public class Client_Integration
    {
        static Task<Source> optimized;

        private static readonly byte[] CopyrightBytes = Encoding.ASCII.GetBytes("Copyright Voormedia");

        [OneTimeSetUp]
        public static void Init()
        {
            var currDir = Directory.GetCurrentDirectory();
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
                var contents = File.ReadAllBytes(file.Path);
                File.WriteAllBytes(@"D:\temp\TestCompressFromFile.png", contents);

                Assert.Greater(size, 1000);
                Assert.Less(size, 1500);

                /* width == 137 */
                Assert.IsTrue(contents.ContainsSubsequence(new byte[] {0, 0, 0, 0x89}));
                Assert.IsFalse(contents.ContainsSubsequence(CopyrightBytes));
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

                File.WriteAllBytes(@"D:\temp\TestCompressFromUrl.png", contents);

                Assert.Greater(size, 1000);
                Assert.Less(size, 1500);

                /* width == 137 */
                Assert.IsTrue(contents.ContainsSubsequence(new byte[] {0, 0, 0, 0x89}));
                Assert.IsFalse(contents.ContainsSubsequence(CopyrightBytes));
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

                File.WriteAllBytes(@"D:\temp\TestShouldResize.png", contents);

                Assert.Greater(size, 500);
                Assert.Less(size, 1000);

                /* width == 50 */
                Assert.IsTrue(contents.ContainsSubsequence(new byte[] {0, 0, 0, 0x32}));
                Assert.IsFalse(contents.ContainsSubsequence(CopyrightBytes));
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
                File.WriteAllBytes(@"D:\temp\TestShouldPreserve.png", contents);

                Assert.Greater(size, 1000);
                Assert.Less(size, 2000);

                /* width == 137 */
                Assert.IsTrue(contents.ContainsSubsequence(new byte[] {0, 0, 0, 0x89}));
                Assert.IsTrue(contents.ContainsSubsequence(CopyrightBytes));
            }
        }
    }
}
