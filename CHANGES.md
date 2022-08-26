## 1.5.3
* Removed expired LetsEncrypt 'DST Root' from cabundle causing validation issues on older systems

## 1.5.2
* Lower TargetFramework to 1.4 to support .NET Core 1.x.

## 1.5.1
* Make Tinify.CompressionCount nullable, it is unset if no calls to the
  server have been made or if the server does not return a count.
* Use POST requests if a body is present, to avoid System.Net.ProtocolViolationException.
* Files read with `Source.FromFile()` will be opened for reading only.

## 1.5.0
* Retry failed requests by default.
* Implementation of .NET API client, compatible with .NET Standard 1.3.
