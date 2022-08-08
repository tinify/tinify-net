using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TinifyAPI
{
    using Method = HttpMethod;

    public class Source
    {
        public static async Task<Source> FromFile(string path)
        {
            var fileBytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
            return await FromBuffer(fileBytes).ConfigureAwait(false);
        }

        public static async Task<Source> FromBuffer(byte[] buffer)
        {
            var response = await Tinify.Client.Request(Method.Post, "/shrink", buffer).ConfigureAwait(false);
            var location = response.Headers.Location;

            return new Source(location);
        }

        public static async Task<Source> FromUrl(string url)
        {
            var body = new Dictionary<string, object> {{"source", new {url}}};

            var response = await Tinify.Client.Request(Method.Post, "/shrink", body).ConfigureAwait(false);
            var location = response.Headers.Location;

            return new Source(location);
        }

        private readonly Uri _url;
        private readonly Dictionary<string, object> _commands;

        internal Source(Uri url, Dictionary<string, object> commands = null)
        {
            _url = url;
            _commands = commands ?? new Dictionary<string, object>();
        }

        public Source Preserve(params string[] options)
        {
            return new Source(_url, MergeCommands("preserve", options));
            }

        public Source Resize(object options)
        {
            return new Source(_url, MergeCommands("resize", options));
        }

        public async Task<ResultMeta> Store(object options)
        {
            var commands = MergeCommands("store", options);
            var response = await Tinify.Client.Request(Method.Post, _url, commands).ConfigureAwait(false);

            return new ResultMeta(response.Headers);
        }

        public async Task<Result> GetResult()
        {
            HttpResponseMessage response;
            if (_commands.Count == 0) {
                response = await Tinify.Client.Request(Method.Get, _url).ConfigureAwait(false);
            } else {
                response = await Tinify.Client.Request(Method.Post, _url, _commands).ConfigureAwait(false);
            }

            var body = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return new Result(response.Headers, response.Content.Headers, body);
        }

        public async Task ToFile(string path) {
            await GetResult().ToFile(path).ConfigureAwait(false);
        }

        public async Task<byte[]> ToBuffer()  {
            return await GetResult().ToBuffer().ConfigureAwait(false);
        }

        private Dictionary<string, object> MergeCommands(string key, object options)
        {
            return new Dictionary<string, object>(_commands) {{key, options}};
        }
    }
}
