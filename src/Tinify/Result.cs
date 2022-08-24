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
            using var file = File.Create(path);
            await file.WriteAsync(Data, 0, Data.Length).ConfigureAwait(false);
        }

        public byte[] ToBuffer() => Data;

        public ulong? Size => (ulong?) Content.ContentLength;

        public string MediaType
        {
            get
            {
                var header = Content.ContentType;
                return header?.MediaType;
            }
        }

        public string ContentType => MediaType;
    }
}
