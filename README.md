# C# File Converter

A powerful, modern file conversion application built with C# and .NET 9, featuring both a beautiful WPF desktop interface and a command-line interface. Convert between various image, audio, and video formats with ease.

![File Converter Screenshot](a.png)

## 🚀 Features

### 🖼️ **Image Conversion**
- **Supported Formats**: JPEG, PNG, BMP, GIF, WebP, TIFF
- **Quality Control**: Adjustable quality settings for lossy formats
- **Batch Processing**: Convert multiple images simultaneously
- **Lossless Processing**: Preserve image quality where possible

### 🎵 **Audio Conversion**
- **Supported Formats**: MP3, WAV, FLAC, AAC, OGG, M4A
- **Quality Settings**: Configurable bitrate and sample rate
- **Metadata Preservation**: Maintain audio tags and metadata
- **Professional Quality**: Powered by FFmpeg

### 🎬 **Video Conversion**
- **Supported Formats**: MP4, AVI, MOV, MKV, WebM, WMV, FLV
- **Advanced Options**: Bitrate, frame rate, and resolution control
- **Codec Selection**: Multiple video and audio codec options
- **Hardware Acceleration**: Optimized performance

### 🎨 **Modern User Interface**
- **WPF Application**: Beautiful Material Design interface
- **Drag & Drop**: Simply drag files into the application
- **Progress Tracking**: Real-time conversion progress bars
- **Batch Operations**: Convert multiple files at once
- **Error Handling**: Clear error messages and status updates

## 📦 Project Structure

```
FileConverter/
├── FileConverter.Core/          # Core conversion library
│   ├── Enums/                  # File type enumerations
│   ├── Interfaces/             # Service interfaces
│   ├── Models/                 # Data models
│   └── Services/               # Conversion services
├── FileConverter.WPF/          # WPF desktop application
│   ├── Behaviors/              # Custom WPF behaviors
│   ├── Converters/             # Value converters
│   ├── Models/                 # WPF-specific models
│   ├── Styles/                 # UI styling
│   ├── ViewModels/             # MVVM view models
│   └── Views/                  # WPF windows and controls
├── FileConverter.CLI/          # Command-line interface
├── FileConverter.Tests/        # Unit and integration tests
└── README.md
```

## 🛠️ Prerequisites

- **.NET 9.0 SDK** or later
- **Windows 10/11** (for WPF application)
- **FFmpeg** (automatically downloaded for audio/video conversions)

## 📥 Installation

### Quick Start (WPF Application)
```bash
# Clone the repository
git clone https://github.com/armendeveloper/c-sharp-file-convertor.git
cd c-sharp-file-convertor

# Build and run the WPF application
dotnet run --project FileConverter.WPF
```

### Using the Build Scripts
```bash
# Windows batch files for easy setup
build-wpf.bat          # Build the WPF application
run-wpf.bat             # Run the WPF application
install-ffmpeg.bat      # Install FFmpeg automatically
```

### Manual Build
```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Run specific projects
dotnet run --project FileConverter.CLI
dotnet run --project FileConverter.WPF
```

## 🎯 Usage

### WPF Desktop Application

1. **Launch the Application**
   ```bash
   dotnet run --project FileConverter.WPF
   ```

2. **Add Files**
   - **Drag & Drop**: Drag files directly into the application window
   - **File Browser**: Click "SELECT FILES" to browse for files
   - **Folder Support**: Drop entire folders to add all supported files

3. **Convert Files**
   - **Individual Conversion**: Click the play button (▶️) next to any file
   - **Batch Conversion**: Click "CONVERT ALL" to process all files
   - **Format Selection**: Choose target formats individually or apply to all

### Command Line Interface

```bash
# Convert a single file
dotnet run --project FileConverter.CLI -- convert input.jpg output.png

# Convert with specific format
dotnet run --project FileConverter.CLI -- convert input.mp4 output.avi --format avi

# Batch convert all files in a directory
dotnet run --project FileConverter.CLI -- batch-convert ./images/ ./output/ --format png

# Show supported formats
dotnet run --project FileConverter.CLI -- list-formats

# Get help
dotnet run --project FileConverter.CLI -- --help
```

### Programmatic Usage

```csharp
using FileConverter.Core.Services;
using FileConverter.Core.Enums;

// Initialize services
var fileTypeDetector = new FileTypeDetector();
var conversionService = new ConversionService(fileTypeDetector);

// Convert a file
var result = await conversionService.ConvertFileAsync(
    "input.jpg", 
    "output.png", 
    FileType.Png
);

if (result.Success)
{
    Console.WriteLine($"Conversion completed: {result.OutputFilePath}");
    Console.WriteLine($"Processing time: {result.ProcessingTime}");
}
else
{
    Console.WriteLine($"Conversion failed: {result.Message}");
}
```

## 🔧 Configuration

### FFmpeg Setup

The application automatically handles FFmpeg installation:

1. **Automatic Download**: FFmpeg is downloaded automatically on first use
2. **System Installation**: Use your package manager:
   ```bash
   # Windows (Chocolatey)
   choco install ffmpeg
   
   # Windows (Winget)
   winget install FFmpeg.FFmpeg
   
   # Or run the provided script
   install-ffmpeg.bat
   ```

### Quality Settings

```csharp
var options = new Dictionary<string, object>
{
    {"quality", 85},      // JPEG/WebP quality (1-100)
    {"bitrate", 192},     // Audio bitrate (kbps)
    {"sampleRate", 48000}, // Audio sample rate (Hz)
    {"videoBitrate", 2000}, // Video bitrate (kbps)
    {"fps", 30}           // Video frame rate
};

var request = new ConversionRequest
{
    InputFilePath = "input.mp4",
    OutputFilePath = "output.mp4",
    TargetFormat = FileType.Mp4,
    Options = options
};
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test categories
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration

# Run tests for specific project
dotnet test FileConverter.Tests
```

## 🏗️ Architecture

### Core Components

- **FileTypeDetector**: Identifies file formats by extension
- **ConversionService**: Orchestrates conversion operations
- **ImageConverter**: Handles image conversions using ImageSharp
- **AudioConverter**: Processes audio files using FFmpeg
- **VideoConverter**: Converts video files using FFmpeg
- **FFmpegService**: Manages FFmpeg installation and configuration

### Design Patterns

- **Dependency Injection**: Clean service registration and resolution
- **Strategy Pattern**: Different converters for different file types
- **MVVM Pattern**: Separation of concerns in WPF application
- **Repository Pattern**: Abstracted data access (future-ready)

## 🤝 Contributing

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/amazing-feature`
3. **Commit your changes**: `git commit -m 'Add amazing feature'`
4. **Push to the branch**: `git push origin feature/amazing-feature`
5. **Open a Pull Request**

### Development Guidelines

- Follow C# coding conventions
- Add unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting

## 📋 Supported File Formats

| Category | Input Formats | Output Formats |
|----------|---------------|----------------|
| **Images** | JPEG, PNG, BMP, GIF, WebP, TIFF | JPEG, PNG, BMP, GIF, WebP, TIFF |
| **Audio** | MP3, WAV, FLAC, AAC, OGG, M4A | MP3, WAV, FLAC, AAC, OGG, M4A |
| **Video** | MP4, AVI, MOV, MKV, WebM, WMV, FLV | MP4, AVI, MOV, MKV, WebM, WMV, FLV |

## 🐛 Troubleshooting

### Common Issues

1. **"FFmpeg not found" error**
   - Run `install-ffmpeg.bat` or install FFmpeg manually
   - Ensure FFmpeg is in your system PATH

2. **"Unsupported file type" error**
   - Check that the file extension is supported
   - Verify the file is not corrupted

3. **Build errors**
   - Ensure .NET 9.0 SDK is installed
   - Run `dotnet restore` to restore packages

### Performance Tips

- **Large Files**: Process large video files individually
- **Batch Operations**: Limit concurrent conversions for better performance
- **Memory Usage**: Close the application between large batch operations

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **ImageSharp**: High-performance image processing library
- **FFMpegCore**: .NET wrapper for FFmpeg
- **Material Design**: Beautiful UI components
- **Community Toolkit**: MVVM helpers and utilities

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/armendeveloper/c-sharp-file-convertor/issues)
- **Discussions**: [GitHub Discussions](https://github.com/armendeveloper/c-sharp-file-convertor/discussions)
- **Email**: analbandyan@servicetitan.com

---

**Built with ❤️ using C# and .NET 9** 