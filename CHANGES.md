* Make Tinify.CompressionCount nullable, it is unset if no calls to the
  server have been made or if the server does not return a count.

## 1.5.0
* Retry failed requests by default.
* Implementation of .NET API client, compatible with .NET Standard 1.3.
