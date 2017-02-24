[<img src="https://travis-ci.org/tinify/tinify-dotnet.svg?branch=master" alt="Build Status">](https://travis-ci.org/tinify/tinify-dotnet)

# Tinify API client for .NET

.NET client for the Tinify API, used for [TinyPNG](https://tinypng.com) and [TinyJPG](https://tinyjpg.com). Tinify compresses your images intelligently. Read more at [http://tinify.com](http://tinify.com).

## Documentation

[Go to the documentation for the .NET client](https://tinypng.com/developers/reference/dotnet).

## Installation

Install the API client:

```
Install-Package Tinify
```

Or add this to your `project.json`:

```json
{
  "dependencies": {
    "Tinify": "*",
  }
}
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

## License

This software is licensed under the MIT License. [View the license](LICENSE).
