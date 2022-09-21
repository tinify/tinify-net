using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace TinifyAPI
{
    using Method = HttpMethod;

    public sealed class Source
    {
        public static async Task<Source> FromFile(string path)
        {
            var buffer = await Task.Run(() => File.ReadAllBytes(path)).ConfigureAwait(false);
            return await FromBuffer(buffer).ConfigureAwait(false);
        }

        public static async Task<Source> FromBuffer(byte[] buffer)
        {
            var response = await Tinify.Client.Request(Method.Post, "/shrink", buffer).ConfigureAwait(false);
            var location = response.Headers.Location;

            return new Source(location);
        }

        public static async Task<Source> FromUrl(string url)
        {
            var body = new Dictionary<string, object> {{"source", new { url = url }}};

            var response = await Tinify.Client.Request(Method.Post, "/shrink", body).ConfigureAwait(false);
            var location = response.Headers.Location;

            return new Source(location);
        }

        private readonly Uri _url;
        private readonly Dictionary<string, object> _commands;

        internal Source(Uri url, Dictionary<string, object> commands = null)
        {
            _url = url;
            commands ??= new Dictionary<string, object>();
            _commands = commands;
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
            var response = await Tinify.Client.Request(Method.Post, _url,
                MergeCommands("store", options)).ConfigureAwait(false);

            return new ResultMeta(response.Headers);
        }

        public Source Convert(object options)
        {
            return new Source(_url, MergeCommands("convert", options));
        }

        public Source TransformBackground(Color backgroundColor)
        {
            var htmlColor = "#" + backgroundColor.R.ToString("X2", null)
                                + backgroundColor.G.ToString("X2", null)
                                + backgroundColor.B.ToString("X2", null);
            return Transform(new {background = htmlColor});
        }

        public Source Transform(object options)
        {
            return new Source(_url, MergeCommands("transform", options));
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
            return new Dictionary<string, object>(this._commands) {{key, options}};
        }
    }
}
