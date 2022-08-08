using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.IO;

namespace TinifyAPI.Internal
{
    internal static class SSL
    {
        public static bool ValidationCallback(HttpRequestMessage req, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable)) return false;
            if (errors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch)) return false;
            return new X509Chain() { ChainPolicy = Policy }.Build(cert);
        }

        private static readonly X509ChainPolicy Policy = CreateSslChainPolicy();

        private static X509ChainPolicy CreateSslChainPolicy()
        {
            const string header = "-----BEGIN CERTIFICATE-----";
            const string footer = "-----END CERTIFICATE-----";

            var policy = new X509ChainPolicy()
            {
                VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority
            };

            using var stream = GetBundleStream();
            using var reader = new StreamReader(stream);
            var pem = reader.ReadToEnd();
            var start = 0;
            while (true)
            {
                start = pem.IndexOf(header, start, StringComparison.Ordinal);
                if (start < 0) break;

                start += header.Length;
                var end = pem.IndexOf(footer, start, StringComparison.Ordinal);
                if (end < 0) break;

                var bytes = Convert.FromBase64String(pem.Substring(start, end - start));
                policy.ExtraStore.Add(new X509Certificate2(bytes));
            }

            return policy;
        }

        private static Stream GetBundleStream() =>
            typeof(SSL).Assembly.GetManifestResourceStream("Tinify.data.cacert.pem");
    }
}
