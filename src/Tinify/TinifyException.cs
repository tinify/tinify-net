namespace TinifyAPI
{
    public class TinifyException : System.Exception {
        internal static TinifyException Create(string message, string type, uint status)
        {
            return status switch
            {
                401 or 429 => new AccountException(message, type, status),
                >= 400 and <= 499 => new ClientException(message, type, status),
                >= 500 and <= 599 => new ServerException(message, type, status),
                _ => new TinifyException(message, type, status)
            };
        }

        public uint Status;

        internal TinifyException() {}

        internal TinifyException(string message, System.Exception err = null) : base(message, err) { }

        internal TinifyException(string message, string type, uint status) :
            base($"{message} (HTTP {status}/{type})")
        {
            Status = status;
        }
    }

    public class AccountException : TinifyException
    {
        internal AccountException() : base() {}

        internal AccountException(string message, System.Exception err = null) : base(message, err) { }

        internal AccountException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ClientException : TinifyException
    {
        internal ClientException() : base() {}

        internal ClientException(string message, System.Exception err = null) : base(message, err) { }

        internal ClientException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ServerException : TinifyException
    {
        internal ServerException() : base() {}

        internal ServerException(string message, System.Exception err = null) : base(message, err) { }

        internal ServerException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ConnectionException : TinifyException
    {
        internal ConnectionException() : base() {}

        internal ConnectionException(string message, System.Exception err = null) : base(message, err) { }

        internal ConnectionException(string message, string type, uint status) : base(message, type, status) { }
    }
}
