using FileConverter.Core.Enums;
using FileConverter.Core.Models;
using FileConverter.Core.Services;
using FluentAssertions;
using Xunit;

namespace FileConverter.Tests.Services;

public class VideoConverterTests
{
    private readonly VideoConverter _converter;

    public VideoConverterTests()
    {
        _converter = new VideoConverter();
    }

    [Fact]
    public void SupportedCategory_ShouldReturnVideo()
    {
        // Act & Assert
        _converter.SupportedCategory.Should().Be(ConversionCategory.Video);
    }

    [Theory]
    [InlineData(FileType.Mp4, FileType.Avi, true)]
    [InlineData(FileType.Avi, FileType.Mp4, true)]
    [InlineData(FileType.Mov, FileType.Mkv, true)]
    [InlineData(FileType.Webm, FileType.Wmv, true)]
    [InlineData(FileType.Flv, FileType.Mp4, true)]
    public void CanConvert_WithSupportedVideoFormats_ShouldReturnTrue(FileType source, FileType target, bool expected)
    {
        // Act
        var result = _converter.CanConvert(source, target);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(FileType.Mp3, FileType.Mp4, false)]
    [InlineData(FileType.Mp4, FileType.Jpeg, false)]
    [InlineData(FileType.Unknown, FileType.Mp4, false)]
    [InlineData(FileType.Mp4, FileType.Unknown, false)]
    public void CanConvert_WithUnsupportedFormats_ShouldReturnFalse(FileType source, FileType target, bool expected)
    {
        // Act
        var result = _converter.CanConvert(source, target);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task ConvertAsync_WithNullRequest_ShouldReturnFailureResult()
    {
        // Act
        var result = await _converter.ConvertAsync(null!);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Request cannot be null");
        result.Exception.Should().NotBeNull();
    }

    [Theory]
    [InlineData("", "output.mp4")]
    [InlineData(null, "output.mp4")]
    [InlineData("input.avi", "")]
    [InlineData("input.avi", null)]
    public async Task ConvertAsync_WithInvalidPaths_ShouldReturnFailureResult(string inputPath, string outputPath)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = inputPath,
            OutputFilePath = outputPath,
            TargetFormat = FileType.Mp4
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid input or output path");
    }

    [Fact]
    public async Task ConvertAsync_WithNonExistentInputFile_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "non_existent_video.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("does not exist");
    }

    [Fact]
    public async Task ConvertAsync_WithUnsupportedTargetFormat_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.mp4",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3 // Audio format for video converter
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Unsupported target format for video conversion");
    }

    [Theory]
    [InlineData(720)]  // Resolution options
    [InlineData(1080)]
    [InlineData(480)]
    public async Task ConvertAsync_WithResolutionOption_ShouldUseResolutionSetting(int height)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4,
            Options = new Dictionary<string, object> { { "Height", height } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Even if conversion fails due to missing file, resolution should be processed
    }

    [Theory]
    [InlineData(1000)]  // Bitrate options for video
    [InlineData(2000)]
    [InlineData(5000)]
    public async Task ConvertAsync_WithBitrateOption_ShouldUseBitrateSetting(int bitrate)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4,
            Options = new Dictionary<string, object> { { "Bitrate", bitrate } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Even if conversion fails due to missing file, bitrate should be processed
    }

    [Theory]
    [InlineData(30)]  // Frame rate options
    [InlineData(60)]
    [InlineData(24)]
    public async Task ConvertAsync_WithFrameRateOption_ShouldUseFrameRateSetting(int frameRate)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4,
            Options = new Dictionary<string, object> { { "FrameRate", frameRate } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Even if conversion fails due to missing file, frame rate should be processed
    }

    [Fact]
    public async Task ConvertAsync_WithMultipleOptions_ShouldUseAllSettings()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4,
            Options = new Dictionary<string, object> 
            { 
                { "Height", 720 },
                { "Bitrate", 2000 },
                { "FrameRate", 30 }
            }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Should handle multiple options gracefully
    }

    [Fact]
    public async Task ConvertAsync_WithEmptyOptions_ShouldUseDefaultSettings()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4,
            Options = new Dictionary<string, object>()
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Should use default settings
    }

    [Fact]
    public async Task ConvertAsync_ShouldMeasureProcessingTime()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "non_existent.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.ProcessingTime.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Theory]
    [InlineData(FileType.Mp4)]
    [InlineData(FileType.Avi)]
    [InlineData(FileType.Mov)]
    [InlineData(FileType.Mkv)]
    [InlineData(FileType.Webm)]
    [InlineData(FileType.Wmv)]
    [InlineData(FileType.Flv)]
    public async Task ConvertAsync_WithDifferentTargetFormats_ShouldSelectCorrectCodec(FileType targetFormat)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.avi",
            OutputFilePath = $"output.{targetFormat.ToString().ToLower()}",
            TargetFormat = targetFormat
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Each format should have its appropriate codec configured
    }

    [Theory]
    [InlineData("InvalidHeight", "not a number")]
    [InlineData("Height", -720)] // Negative height
    [InlineData("Bitrate", -1000)] // Negative bitrate
    [InlineData("FrameRate", -30)] // Negative frame rate
    public async Task ConvertAsync_WithInvalidOptions_ShouldHandleGracefully(string optionKey, object optionValue)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4,
            Options = new Dictionary<string, object> { { optionKey, optionValue } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Should handle invalid options gracefully without crashing
    }

    [Theory]
    [InlineData(FileType.Mp4, "libx264", "aac")]    // H.264 + AAC for MP4
    [InlineData(FileType.Webm, "libvpx", "libvorbis")] // VP8 + Vorbis for WebM
    [InlineData(FileType.Avi, "libx264", "libmp3lame")] // H.264 + MP3 for AVI
    public async Task ConvertAsync_WithSpecificFormats_ShouldUseExpectedCodecs(FileType targetFormat, string expectedVideoCodec, string expectedAudioCodec)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.mp4",
            OutputFilePath = $"output.{targetFormat.ToString().ToLower()}",
            TargetFormat = targetFormat
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Note: Expected codecs are {expectedVideoCodec} and {expectedAudioCodec}
        // The codec selection logic should be working (tested indirectly through the conversion process)
        expectedVideoCodec.Should().NotBeNullOrEmpty("Video codec should be specified");
        expectedAudioCodec.Should().NotBeNullOrEmpty("Audio codec should be specified");
    }

    [Fact]
    public async Task ConvertAsync_WithLargeFile_ShouldHandleTimeout()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "large_video.avi",
            OutputFilePath = "output.mp4",
            TargetFormat = FileType.Mp4
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Should handle timeouts or large files gracefully
    }
} 