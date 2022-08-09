using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;

namespace TinifyAPI
{
    public class Result : ResultMeta
    {
        protected HttpContentHeaders Content { get; set; }
        protected byte[] Data { get; set; }

        internal Result(HttpResponseHeaders meta, HttpContentHeaders content, byte[] data) : base(meta)
        {
            Content = content;
            Data = data;
        }

        public async Task ToFile(string path)
        {
            await File.WriteAllBytesAsync(path, Data).ConfigureAwait(false);
        }

        public byte[] ToBuffer()
        {
            return Data;
        }

        public ulong? Size => (ulong?) Content.ContentLength;

        public string MediaType => Content.ContentType?.MediaType;

        public string ContentType => MediaType;
    }
}
