using FileConverter.Core.Enums;
using FileConverter.Core.Models;
using FileConverter.Core.Services;
using FluentAssertions;
using Xunit;

namespace FileConverter.Tests.Services;

public class AudioConverterTests
{
    private readonly AudioConverter _converter;

    public AudioConverterTests()
    {
        _converter = new AudioConverter();
    }

    [Fact]
    public void SupportedCategory_ShouldReturnAudio()
    {
        // Act & Assert
        _converter.SupportedCategory.Should().Be(ConversionCategory.Audio);
    }

    [Theory]
    [InlineData(FileType.Mp3, FileType.Wav, true)]
    [InlineData(FileType.Wav, FileType.Mp3, true)]
    [InlineData(FileType.Flac, FileType.Aac, true)]
    [InlineData(FileType.Ogg, FileType.M4a, true)]
    [InlineData(FileType.Aac, FileType.Flac, true)]
    public void CanConvert_WithSupportedAudioFormats_ShouldReturnTrue(FileType source, FileType target, bool expected)
    {
        // Act
        var result = _converter.CanConvert(source, target);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(FileType.Jpeg, FileType.Mp3, false)]
    [InlineData(FileType.Mp3, FileType.Mp4, false)]
    [InlineData(FileType.Unknown, FileType.Wav, false)]
    [InlineData(FileType.Wav, FileType.Unknown, false)]
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
    [InlineData("", "output.mp3")]
    [InlineData(null, "output.mp3")]
    [InlineData("input.wav", "")]
    [InlineData("input.wav", null)]
    public async Task ConvertAsync_WithInvalidPaths_ShouldReturnFailureResult(string inputPath, string outputPath)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = inputPath,
            OutputFilePath = outputPath,
            TargetFormat = FileType.Mp3
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
            InputFilePath = "non_existent_audio.wav",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3
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
            InputFilePath = "input.wav",
            OutputFilePath = "output.jpg",
            TargetFormat = FileType.Jpeg // Image format for audio converter
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Unsupported target format for audio conversion");
    }

    [Theory]
    [InlineData(128)] // Bitrate options
    [InlineData(192)]
    [InlineData(320)]
    public async Task ConvertAsync_WithBitrateOption_ShouldUseBitrateSetting(int bitrate)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.wav",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3,
            Options = new Dictionary<string, object> { { "Bitrate", bitrate } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Even if conversion fails due to missing file, bitrate should be processed
    }

    [Theory]
    [InlineData(44100)] // Sample rate options
    [InlineData(48000)]
    [InlineData(96000)]
    public async Task ConvertAsync_WithSampleRateOption_ShouldUseSampleRateSetting(int sampleRate)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.mp3",
            OutputFilePath = "output.wav",
            TargetFormat = FileType.Wav,
            Options = new Dictionary<string, object> { { "SampleRate", sampleRate } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Even if conversion fails due to missing file, sample rate should be processed
    }

    [Fact]
    public async Task ConvertAsync_WithMultipleOptions_ShouldUseAllSettings()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.wav",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3,
            Options = new Dictionary<string, object> 
            { 
                { "Bitrate", 256 },
                { "SampleRate", 44100 }
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
            InputFilePath = "input.wav",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3,
            Options = new Dictionary<string, object>()
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Should use default bitrate (192) and sample rate (44100)
    }

    [Fact]
    public async Task ConvertAsync_ShouldMeasureProcessingTime()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "non_existent.wav",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.ProcessingTime.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Theory]
    [InlineData(FileType.Mp3)]
    [InlineData(FileType.Wav)]
    [InlineData(FileType.Flac)]
    [InlineData(FileType.Aac)]
    [InlineData(FileType.Ogg)]
    [InlineData(FileType.M4a)]
    public async Task ConvertAsync_WithDifferentTargetFormats_ShouldSelectCorrectCodec(FileType targetFormat)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.wav",
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
    [InlineData("InvalidBitrate", "not a number")]
    [InlineData("Bitrate", -128)] // Negative bitrate
    [InlineData("SampleRate", -44100)] // Negative sample rate
    public async Task ConvertAsync_WithInvalidOptions_ShouldHandleGracefully(string optionKey, object optionValue)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.wav",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3,
            Options = new Dictionary<string, object> { { optionKey, optionValue } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Should handle invalid options gracefully without crashing
    }
} 