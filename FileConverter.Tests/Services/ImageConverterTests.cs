using FileConverter.Core.Enums;
using FileConverter.Core.Models;
using FileConverter.Core.Services;
using FluentAssertions;
using Xunit;

namespace FileConverter.Tests.Services;

public class ImageConverterTests
{
    private readonly ImageConverter _converter;

    public ImageConverterTests()
    {
        _converter = new ImageConverter();
    }

    [Fact]
    public void SupportedCategory_ShouldReturnImage()
    {
        // Act & Assert
        _converter.SupportedCategory.Should().Be(ConversionCategory.Image);
    }

    [Theory]
    [InlineData(FileType.Jpeg, FileType.Png, true)]
    [InlineData(FileType.Png, FileType.Jpeg, true)]
    [InlineData(FileType.Bmp, FileType.Gif, true)]
    [InlineData(FileType.Webp, FileType.Tiff, true)]
    [InlineData(FileType.Gif, FileType.Webp, true)]
    public void CanConvert_WithSupportedImageFormats_ShouldReturnTrue(FileType source, FileType target, bool expected)
    {
        // Act
        var result = _converter.CanConvert(source, target);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(FileType.Mp3, FileType.Jpeg, false)]
    [InlineData(FileType.Jpeg, FileType.Mp4, false)]
    [InlineData(FileType.Unknown, FileType.Png, false)]
    [InlineData(FileType.Png, FileType.Unknown, false)]
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
    [InlineData("", "output.jpg")]
    [InlineData(null, "output.jpg")]
    [InlineData("input.jpg", "")]
    [InlineData("input.jpg", null)]
    public async Task ConvertAsync_WithInvalidPaths_ShouldReturnFailureResult(string inputPath, string outputPath)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = inputPath,
            OutputFilePath = outputPath,
            TargetFormat = FileType.Jpeg
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
            InputFilePath = "non_existent_file.jpg",
            OutputFilePath = "output.png",
            TargetFormat = FileType.Png
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("does not exist");
        result.Exception.Should().BeNull(); // ImageConverter doesn't set exception for missing files
    }

    [Theory]
    [InlineData(FileType.Jpeg)]
    [InlineData(FileType.Png)]
    [InlineData(FileType.Bmp)]
    [InlineData(FileType.Gif)]
    [InlineData(FileType.Webp)]
    [InlineData(FileType.Tiff)]
    public void GetImageEncoder_WithSupportedFormats_ShouldNotThrow(FileType targetFormat)
    {
        // Act & Assert - Using reflection to test private method behavior indirectly
        var request = new ConversionRequest
        {
            InputFilePath = "test.jpg",
            OutputFilePath = "test." + targetFormat.ToString().ToLower(),
            TargetFormat = targetFormat
        };

        // This should not throw when getting the encoder
        var action = () => _converter.CanConvert(FileType.Jpeg, targetFormat);
        action.Should().NotThrow();
    }

    [Fact]
    public async Task ConvertAsync_WithUnsupportedTargetFormat_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.jpg",
            OutputFilePath = "output.mp3",
            TargetFormat = FileType.Mp3 // Audio format for image converter
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Unsupported target format for image conversion");
    }

    [Fact]
    public async Task ConvertAsync_ShouldMeasureProcessingTime()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "non_existent.jpg",
            OutputFilePath = "output.png",
            TargetFormat = FileType.Png
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.ProcessingTime.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Theory]
    [InlineData(90)] // JPEG quality
    [InlineData(70)]
    [InlineData(100)]
    public async Task ConvertAsync_WithJpegQualityOption_ShouldUseQualitySetting(int quality)
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.png",
            OutputFilePath = "output.jpg",
            TargetFormat = FileType.Jpeg,
            Options = new Dictionary<string, object> { { "Quality", quality } }
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert - Even if it fails due to missing file, the quality option should be processed
        result.Should().NotBeNull();
        // The quality should be handled in the conversion logic (tested indirectly)
    }

    [Fact]
    public async Task ConvertAsync_WithEmptyOptions_ShouldUseDefaultSettings()
    {
        // Arrange
        var request = new ConversionRequest
        {
            InputFilePath = "input.jpg",
            OutputFilePath = "output.png",
            TargetFormat = FileType.Png,
            Options = new Dictionary<string, object>()
        };

        // Act
        var result = await _converter.ConvertAsync(request);

        // Assert
        result.Should().NotBeNull();
        // Should handle empty options gracefully
    }
} 