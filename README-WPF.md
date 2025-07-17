# File Converter WPF Application

A modern, user-friendly Windows desktop application for converting files between different formats. Built with WPF and Material Design for a beautiful and intuitive user experience.

## Features

### 🖼️ **Image Conversion**
- **Supported Formats**: JPEG, PNG, BMP, GIF, WebP, TIFF
- **Quality Control**: Adjustable quality settings for lossy formats
- **Batch Processing**: Convert multiple images at once

### 🎵 **Audio Conversion**
- **Supported Formats**: MP3, WAV, FLAC, AAC, OGG, M4A
- **Quality Settings**: Configurable bitrate and sample rate
- **FFmpeg Integration**: Professional-grade audio processing

### 🎬 **Video Conversion**
- **Supported Formats**: MP4, AVI, MOV, MKV, WebM, WMV, FLV
- **Advanced Options**: Bitrate, frame rate, and resolution control
- **Hardware Acceleration**: Optimized for performance

### 🎨 **Modern UI Features**
- **Drag & Drop**: Simply drag files into the application
- **Material Design**: Beautiful, modern interface
- **Progress Tracking**: Real-time conversion progress
- **Batch Operations**: Convert multiple files simultaneously
- **Format Detection**: Automatic file type recognition
- **Error Handling**: Clear error messages and status updates

## Prerequisites

- **Windows 10/11** with .NET 9.0 Runtime
- **FFmpeg** (automatically handled by the application)
- **At least 4GB RAM** recommended for video conversions

## Installation & Usage

### Quick Start
1. Clone or download this repository
2. Double-click `build-wpf.bat` to build the application
3. Double-click `run-wpf.bat` to start the application

### Manual Build
```bash
# Restore dependencies
dotnet restore FileConverter.WPF/FileConverter.WPF.csproj

# Build the application
dotnet build FileConverter.WPF/FileConverter.WPF.csproj --configuration Release

# Run the application
dotnet run --project FileConverter.WPF
```

## How to Use

### 1. **Adding Files**
- **Drag & Drop**: Drag files or folders directly into the application window
- **File Browser**: Click "SELECT FILES" to browse and select files
- **Folder Support**: Drop entire folders to automatically add all supported files

### 2. **Setting Output Formats**
- **Individual Files**: Use the dropdown next to each file to set its target format
- **Batch Setting**: Use the "Apply to all" section to set the same format for all files of a category
- **Auto-Detection**: Files are automatically categorized (Image/Audio/Video)

### 3. **Converting Files**
- **Single File**: Click the play button (▶️) next to any file
- **Batch Conversion**: Click "CONVERT ALL" to process all pending files
- **Progress Tracking**: Watch real-time progress bars and status updates

### 4. **Managing Results**
- **Open Output**: Click the folder icon (📁) to open the converted file location
- **Remove Files**: Click the X button to remove files from the list
- **Clear Completed**: Remove all successfully converted files
- **Clear All**: Remove all files from the list

## Advanced Features

### Quality Settings
The application supports quality settings for various formats:
- **JPEG/WebP**: Quality percentage (1-100)
- **Audio**: Bitrate and sample rate options
- **Video**: Bitrate, frame rate, and resolution controls

### Supported File Extensions

#### Images
`.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.webp`, `.tiff`

#### Audio
`.mp3`, `.wav`, `.flac`, `.aac`, `.ogg`, `.m4a`

#### Video
`.mp4`, `.avi`, `.mov`, `.mkv`, `.webm`, `.wmv`, `.flv`

## Technical Details

### Architecture
- **Frontend**: WPF with Material Design
- **Backend**: .NET 9.0 with async/await patterns
- **Image Processing**: SixLabors.ImageSharp
- **Audio/Video**: FFMpegCore
- **MVVM Pattern**: CommunityToolkit.Mvvm

### Performance
- **Multi-threading**: Parallel processing with configurable concurrency
- **Memory Efficient**: Streaming processing for large files
- **Progress Reporting**: Real-time status updates
- **Error Recovery**: Graceful handling of conversion failures

## Troubleshooting

### Common Issues

1. **"Unsupported file type" error**
   - Ensure the file extension is in the supported list
   - Check if the file is corrupted

2. **Conversion fails for video/audio files**
   - Verify FFmpeg is available (automatically handled)
   - Check available disk space

3. **Application won't start**
   - Ensure .NET 9.0 Runtime is installed
   - Run `dotnet --version` to verify installation

### Getting Help
- Check the status bar for detailed error messages
- Look at the processing time to identify slow conversions
- Use the individual file conversion to isolate issues

## License

This project is part of the File Converter suite. See the main README for license information.

## Contributing

Contributions are welcome! Please see the main project README for contribution guidelines. 