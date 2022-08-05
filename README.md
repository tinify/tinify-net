[![Build status](https://ci.appveyor.com/api/projects/status/9pgat54tma1kdko8?svg=true)](https://ci.appveyor.com/project/jshergal/tinify-net-standard2-0)

# Tinify API client for .NET

.NET client for the Tinify API, used for [TinyPNG](https://tinypng.com) and [TinyJPG](https://tinyjpg.com). Tinify compresses your images intelligently. Read more at [http://tinify.com](http://tinify.com).

## Documentation

[Go to the documentation for the .NET client](https://tinypng.com/developers/reference/dotnet).

## Installation

Install the API client:

```
Install-Package Tinify-netstandard20
```

## Usage

```csharp
using TinifyAPI;

class Compress
{
  static void Main()
  {
    Tinify.Key = "YOUR_API_KEY";
    Tinify.FromFile("unoptimized.png").ToFile("optimized.png").Wait();
  }
}
```

## Running tests

```
dotnet restore
dotnet test test/Tinify.Tests
```

### Integration tests

```
dotnet restore
TINIFY_KEY=$YOUR_API_KEY dotnet test test/Tinify.Tests.Integration
```
Or add a `.env` file to the `/test/Tinify.Tests.Integration` directory in the format
```
TINIFY_KEY=<YOUR_API_KEY>
```

## License

This software is licensed under the MIT License. [View the license](LICENSE).
