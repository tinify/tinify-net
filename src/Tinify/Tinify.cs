using System.Net.Http;
using System.Threading.Tasks;

/* We cannot and should not give a namespace and class the same name:
   https://msdn.microsoft.com/en-us/library/ms229026(v=vs.110).aspx */
namespace TinifyAPI
{
    using Method = HttpMethod;

    public class Tinify
    {
        private static readonly object Mutex = new();
        private static Client _client;

        private static string _key;
        private static string _appIdentifier;
        private static string _proxy;

        public static string Key
        {
            get => _key;

            set
            {
                _key = value;
                ResetClient();
            }
        }

        public static string AppIdentifier
        {
            get => _appIdentifier;

            set
            {
                _appIdentifier = value;
                ResetClient();
            }
        }

        public static string Proxy
        {
            get => _proxy;

            set
            {
                _proxy = value;
                ResetClient();
            }
        }

        private static void ResetClient()
        {
            _client?.Dispose();
            _client = null;
        }

        public static uint? CompressionCount { get; set; }

        public static Client Client
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_key))
                {
                    throw new AccountException("Provide an API key with Tinify.Key = ...");
                }

                if (_client != null)
                {
                    return _client;
                }
                else
                {
                    lock (Mutex)
                    {
                        _client ??= new Client(_key, _appIdentifier, _proxy);
                    }
                    return _client;
                }
            }
        }

        public static Task<Source> FromFile(string path)
        {
            return Source.FromFile(path);
        }

        public static Task<Source> FromBuffer(byte[] buffer)
        {
            return Source.FromBuffer(buffer);
        }

        public static Task<Source> FromUrl(string url)
        {
            return Source.FromUrl(url);
        }

        public static async Task<bool> Validate()
        {
            try
            {
                await Client.Request(Method.Post, "/shrink").ConfigureAwait(false);
            }
            catch (AccountException err)
            {
                if (err.Status == 429)
                {
                    return true;
                }
                throw;
            }
            catch (ClientException)
            {
                return true;
            }
            return false;
        }
    }
}
