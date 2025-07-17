# FileConverter

A comprehensive C# solution for converting between different file formats, supporting images, audio, and video files.

## Features

### Supported File Types

**Images:**
- JPEG (.jpg, .jpeg)
- PNG (.png)
- BMP (.bmp)
- GIF (.gif)
- WebP (.webp)
- TIFF (.tiff, .tif)

**Audio:**
- MP3 (.mp3)
- WAV (.wav)
- FLAC (.flac)
- AAC (.aac)
- OGG (.ogg)
- M4A (.m4a)

**Video:**
- MP4 (.mp4)
- AVI (.avi)
- MOV (.mov)
- MKV (.mkv)
- WebM (.webm)
- WMV (.wmv)
- FLV (.flv)

## Prerequisites

### For Audio and Video Conversion
- **FFmpeg** must be installed and accessible in your system PATH
- Download from: https://ffmpeg.org/download.html

### .NET Requirements
- .NET 8.0 or later

## Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd FileConverter
```

2. Build the solution:
```bash
dotnet build
```

3. (Optional) Create a global tool:
```bash
dotnet pack FileConverter.CLI
dotnet tool install --global --add-source ./FileConverter.CLI/nupkg FileConverter.CLI
```

## Usage

### Command Line Interface

#### Basic Usage
```bash
# Convert image.png to image.jpg
dotnet run --project FileConverter.CLI -- image.png image.jpg

# Convert audio.wav to audio.mp3
dotnet run --project FileConverter.CLI -- -i audio.wav -o audio.mp3

# Convert video with specific format
dotnet run --project FileConverter.CLI -- video.avi video.mp4 -f Mp4
```

#### Command Line Options
```
FileConverter [options] <input> <output>

Options:
  -i, --input <path>     Input file path
  -o, --output <path>    Output file path
  -f, --format <format>  Target format (optional, auto-detected from output extension)
  --formats              Show supported formats
  -h, --help             Show help message
```

#### Examples
```bash
# Simple conversion (format detected from extension)
FileConverter image.png image.jpg
FileConverter audio.wav audio.mp3
FileConverter video.avi video.mp4

# Using explicit parameters
FileConverter -i input.bmp -o output.png
FileConverter --input audio.flac --output audio.aac

# Show all supported formats
FileConverter --formats

# Get help
FileConverter --help
```

### Programmatic Usage

#### Basic Conversion
```csharp
using FileConverter.Core.Services;
using FileConverter.Core.Interfaces;

// Setup services
IFileTypeDetector detector = new FileTypeDetector();
IConversionService converter = new ConversionService(detector);

// Convert a file
var result = await converter.ConvertFileAsync("input.png", "output.jpg");

if (result.Success)
{
    Console.WriteLine($"Conversion successful: {result.Message}");
    Console.WriteLine($"Processing time: {result.ProcessingTime}");
}
else
{
    Console.WriteLine($"Conversion failed: {result.Message}");
}
```

#### Advanced Options
```csharp
using FileConverter.Core.Models;
using FileConverter.Core.Enums;

// Create a conversion request with options
var request = new ConversionRequest
{
    InputFilePath = "input.jpg",
    OutputFilePath = "output.png",
    TargetFormat = FileType.Png,
    Options = new Dictionary<string, object>
    {
        ["quality"] = 90  // For JPEG/WebP
    }
};

// For audio/video, you can specify additional options:
var audioRequest = new ConversionRequest
{
    InputFilePath = "input.wav",
    OutputFilePath = "output.mp3",
    TargetFormat = FileType.Mp3,
    Options = new Dictionary<string, object>
    {
        ["bitrate"] = 192,        // Audio bitrate in kbps
        ["sampleRate"] = 44100    // Sample rate in Hz
    }
};

var videoRequest = new ConversionRequest
{
    InputFilePath = "input.avi",
    OutputFilePath = "output.mp4",
    TargetFormat = FileType.Mp4,
    Options = new Dictionary<string, object>
    {
        ["videoBitrate"] = 2000,  // Video bitrate in kbps
        ["audioBitrate"] = 128,   // Audio bitrate in kbps
        ["fps"] = 30,             // Frames per second
        ["width"] = 1920,         // Video width
        ["height"] = 1080         // Video height
    }
};
```

## Architecture

### Project Structure
```
FileConverter/
├── FileConverter.Core/          # Core conversion logic
│   ├── Enums/                   # File type definitions
│   ├── Interfaces/              # Service contracts
│   ├── Models/                  # Data models
│   └── Services/                # Implementation services
├── FileConverter.CLI/           # Command-line interface
└── FileConverter.sln           # Solution file
```

### Core Components

#### Services
- **ConversionService**: Main orchestrator for file conversions
- **FileTypeDetector**: Detects file types from extensions
- **ImageConverter**: Handles image format conversions using ImageSharp
- **AudioConverter**: Handles audio format conversions using FFMpegCore
- **VideoConverter**: Handles video format conversions using FFMpegCore

#### Models
- **ConversionRequest**: Represents a conversion request with options
- **ConversionResult**: Contains the result of a conversion operation

#### Dependencies
- **FFMpegCore**: Audio and video processing
- **SixLabors.ImageSharp**: Image processing
- **Microsoft.Extensions.Hosting**: Dependency injection and hosting

## Error Handling

The application provides comprehensive error handling:

- **File not found**: Clear error messages for missing input files
- **Unsupported formats**: Validation of supported file types
- **FFmpeg issues**: Detailed error reporting for audio/video conversion problems
- **Processing errors**: Exception details and processing time information

## Performance Considerations

- **Asynchronous processing**: All conversions are performed asynchronously
- **Memory efficient**: Streaming processing for large files
- **Progress tracking**: Processing time measurement for all operations

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Troubleshooting

### FFmpeg Not Found
If you encounter errors with audio/video conversion:
1. Ensure FFmpeg is installed on your system
2. Verify FFmpeg is in your system PATH
3. Test FFmpeg installation: `ffmpeg -version`

### Unsupported Format
- Check the supported formats list using `--formats`
- Ensure file extensions match the actual file content
- Verify the target format is supported for the source file category

### Performance Issues
- For large video files, consider adjusting bitrate and resolution options
- Monitor system resources during conversion
- Use appropriate output formats for your use case 