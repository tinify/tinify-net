using System;
using System.Net;

namespace TinifyAPI.Internal
{
    internal class Proxy : IWebProxy
    {
        private Uri Uri { get; }

        public ICredentials Credentials { get; set; }

        public Proxy(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                throw new ConnectionException(
                    $"Invalid proxy: cannot parse '{url}'"
                );
            }

            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                var user = uri.UserInfo.Split(':');
                Credentials = new NetworkCredential(user[0], user[1]);
            }

            Uri = uri;
        }

        public Uri GetProxy(Uri destination)
        {
            return Uri;
        }

        public bool IsBypassed(Uri host)
        {
            return host.IsLoopback;
        }
    }
}
