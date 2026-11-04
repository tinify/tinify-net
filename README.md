[![MIT License](http://img.shields.io/badge/license-MIT-green.svg) ](https://github.com/tinify/tinify-python/blob/main/LICENSE)
[![CI_CD](https://github.com/tinify/tinify-net/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/tinify/tinify-net/actions/workflows/ci-cd.yml)
[![Nuget](https://img.shields.io/nuget/v/Tinify)](https://www.nuget.org/packages/Tinify)

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

Convert to another format with the `Format` constants (`WebP`, `Png`,
`Jpeg`, `Jpg`, `Avif`, `Jxl` and `Any`):

```csharp
Tinify.FromFile("unoptimized.png")
    .Convert(new { type = Format.Jxl })
    .ToFile("optimized.jxl")
    .Wait();
```

Pass several and the API returns the smallest result:

```csharp
var result = await Tinify.FromFile("unoptimized.png")
    .Convert(new { type = new[] { Format.Jxl, Format.WebP } })
    .GetResult();

await result.ToFile($"optimized.{result.Extension}");
```

The constants are plain strings, so `type = "image/jxl"` keeps working.

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
