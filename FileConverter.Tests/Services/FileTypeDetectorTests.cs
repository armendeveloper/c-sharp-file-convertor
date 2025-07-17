using FileConverter.Core.Enums;
using FileConverter.Core.Services;
using FluentAssertions;
using Xunit;

namespace FileConverter.Tests.Services;

public class FileTypeDetectorTests
{
    private readonly FileTypeDetector _detector;

    public FileTypeDetectorTests()
    {
        _detector = new FileTypeDetector();
    }

    [Theory]
    [InlineData("image.jpg", FileType.Jpeg)]
    [InlineData("image.jpeg", FileType.Jpeg)]
    [InlineData("IMAGE.JPG", FileType.Jpeg)] // Case insensitive
    [InlineData("image.png", FileType.Png)]
    [InlineData("image.bmp", FileType.Bmp)]
    [InlineData("image.gif", FileType.Gif)]
    [InlineData("image.webp", FileType.Webp)]
    [InlineData("image.tiff", FileType.Tiff)]
    [InlineData("image.tif", FileType.Tiff)]
    public void DetectFileType_WithImageExtensions_ShouldReturnCorrectImageType(string fileName, FileType expectedType)
    {
        // Act
        var result = _detector.DetectFileType(fileName);

        // Assert
        result.Should().Be(expectedType);
    }

    [Theory]
    [InlineData("audio.mp3", FileType.Mp3)]
    [InlineData("audio.wav", FileType.Wav)]
    [InlineData("audio.flac", FileType.Flac)]
    [InlineData("audio.aac", FileType.Aac)]
    [InlineData("audio.ogg", FileType.Ogg)]
    [InlineData("audio.m4a", FileType.M4a)]
    [InlineData("AUDIO.MP3", FileType.Mp3)] // Case insensitive
    public void DetectFileType_WithAudioExtensions_ShouldReturnCorrectAudioType(string fileName, FileType expectedType)
    {
        // Act
        var result = _detector.DetectFileType(fileName);

        // Assert
        result.Should().Be(expectedType);
    }

    [Theory]
    [InlineData("video.mp4", FileType.Mp4)]
    [InlineData("video.avi", FileType.Avi)]
    [InlineData("video.mov", FileType.Mov)]
    [InlineData("video.mkv", FileType.Mkv)]
    [InlineData("video.webm", FileType.Webm)]
    [InlineData("video.wmv", FileType.Wmv)]
    [InlineData("video.flv", FileType.Flv)]
    [InlineData("VIDEO.MP4", FileType.Mp4)] // Case insensitive
    public void DetectFileType_WithVideoExtensions_ShouldReturnCorrectVideoType(string fileName, FileType expectedType)
    {
        // Act
        var result = _detector.DetectFileType(fileName);

        // Assert
        result.Should().Be(expectedType);
    }

    [Theory]
    [InlineData("file.txt")]
    [InlineData("file.doc")]
    [InlineData("file.unknown")]
    [InlineData("file")]
    [InlineData("")]
    [InlineData(null)]
    public void DetectFileType_WithUnsupportedExtensions_ShouldReturnUnknown(string fileName)
    {
        // Act
        var result = _detector.DetectFileType(fileName);

        // Assert
        result.Should().Be(FileType.Unknown);
    }

    [Theory]
    [InlineData(FileType.Jpeg, ConversionCategory.Image)]
    [InlineData(FileType.Png, ConversionCategory.Image)]
    [InlineData(FileType.Mp3, ConversionCategory.Audio)]
    [InlineData(FileType.Wav, ConversionCategory.Audio)]
    [InlineData(FileType.Mp4, ConversionCategory.Video)]
    [InlineData(FileType.Avi, ConversionCategory.Video)]
    public void GetCategory_WithKnownFileTypes_ShouldReturnCorrectCategory(FileType fileType, ConversionCategory expectedCategory)
    {
        // Act
        var result = _detector.GetCategory(fileType);

        // Assert
        result.Should().Be(expectedCategory);
    }

    [Fact]
    public void GetCategory_WithUnknownFileType_ShouldThrowArgumentException()
    {
        // Act & Assert
        _detector.Invoking(d => d.GetCategory(FileType.Unknown))
            .Should().Throw<ArgumentException>()
            .WithMessage("Unsupported file type: Unknown");
    }

    [Theory]
    [InlineData("C:\\path\\to\\file.jpg", FileType.Jpeg)]
    [InlineData("/path/to/file.png", FileType.Png)]
    [InlineData("relative/path/file.mp3", FileType.Mp3)]
    public void DetectFileType_WithFullPaths_ShouldDetectFromExtension(string filePath, FileType expectedType)
    {
        // Act
        var result = _detector.DetectFileType(filePath);

        // Assert
        result.Should().Be(expectedType);
    }
} 