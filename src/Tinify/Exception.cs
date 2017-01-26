namespace TinifyAPI
{
    public class Exception : System.Exception {
        internal static Exception Create(string message, string type, uint status) {
            if (status == 401 || status == 429)
            {
                return new AccountException(message, type, status);
            }
            else if (status >= 400 && status <= 499)
            {
                return new ClientException(message, type, status);
            }
            else if (status >= 500 && status <= 599)
            {
                return new ServerException(message, type, status);
            }
            else
            {
                return new Exception(message, type, status);
            }
        }

        public uint Status = 0;

        internal Exception() : base() {}

        internal Exception(string message, System.Exception err = null) : base(message, err) { }

        internal Exception(string message, string type, uint status) :
            base(message + " (HTTP " + status + "/" + type + ")")
        {
            this.Status = status;
        }
    }

    public class AccountException : Exception
    {
        internal AccountException() : base() {}

        internal AccountException(string message, System.Exception err = null) : base(message, err) { }

        internal AccountException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ClientException : Exception
    {
        internal ClientException() : base() {}

        internal ClientException(string message, System.Exception err = null) : base(message, err) { }

        internal ClientException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ServerException : Exception
    {
        internal ServerException() : base() {}

        internal ServerException(string message, System.Exception err = null) : base(message, err) { }

        internal ServerException(string message, string type, uint status) : base(message, type, status) { }
    }

    public class ConnectionException : Exception
    {
        internal ConnectionException() : base() {}

        internal ConnectionException(string message, System.Exception err = null) : base(message, err) { }

        internal ConnectionException(string message, string type, uint status) : base(message, type, status) { }
    }
}
