using FileConverter.Core.Enums;
using FileConverter.Core.Models;
using FluentAssertions;
using Xunit;

namespace FileConverter.Tests.Models;

public class ModelTests
{
    [Fact]
    public void ConversionRequest_DefaultInitialization_ShouldHaveEmptyValues()
    {
        // Act
        var request = new ConversionRequest();

        // Assert
        request.InputFilePath.Should().Be(string.Empty);
        request.OutputFilePath.Should().Be(string.Empty);
        request.TargetFormat.Should().Be(FileType.Unknown);
        request.Options.Should().NotBeNull();
        request.Options.Should().BeEmpty();
    }

    [Fact]
    public void ConversionRequest_WithInitializedProperties_ShouldRetainValues()
    {
        // Arrange & Act
        var request = new ConversionRequest
        {
            InputFilePath = "input.jpg",
            OutputFilePath = "output.png",
            TargetFormat = FileType.Png,
            Options = new Dictionary<string, object> { { "Quality", 90 } }
        };

        // Assert
        request.InputFilePath.Should().Be("input.jpg");
        request.OutputFilePath.Should().Be("output.png");
        request.TargetFormat.Should().Be(FileType.Png);
        request.Options.Should().HaveCount(1);
        request.Options["Quality"].Should().Be(90);
    }

    [Fact]
    public void ConversionRequest_Options_ShouldAcceptVariousDataTypes()
    {
        // Arrange & Act
        var request = new ConversionRequest
        {
            Options = new Dictionary<string, object>
            {
                { "IntOption", 100 },
                { "StringOption", "test" },
                { "BoolOption", true },
                { "DoubleOption", 3.14 }
            }
        };

        // Assert
        request.Options["IntOption"].Should().Be(100);
        request.Options["StringOption"].Should().Be("test");
        request.Options["BoolOption"].Should().Be(true);
        request.Options["DoubleOption"].Should().Be(3.14);
    }

    [Fact]
    public void ConversionRequest_Options_ShouldHandleNullValues()
    {
        // Arrange & Act
        var request = new ConversionRequest
        {
            Options = new Dictionary<string, object>
            {
                { "NullOption", null! }
            }
        };

        // Assert
        request.Options.Should().ContainKey("NullOption");
        request.Options["NullOption"].Should().BeNull();
    }

    [Fact]
    public void ConversionResult_DefaultInitialization_ShouldHaveDefaultValues()
    {
        // Act
        var result = new ConversionResult();

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be(string.Empty);
        result.OutputFilePath.Should().Be(string.Empty);
        result.ProcessingTime.Should().Be(TimeSpan.Zero);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void ConversionResult_WithInitializedProperties_ShouldRetainValues()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var processingTime = TimeSpan.FromSeconds(5.5);

        // Act
        var result = new ConversionResult
        {
            Success = true,
            Message = "Conversion successful",
            OutputFilePath = "output.jpg",
            ProcessingTime = processingTime,
            Exception = exception
        };

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Conversion successful");
        result.OutputFilePath.Should().Be("output.jpg");
        result.ProcessingTime.Should().Be(processingTime);
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void ConversionResult_SuccessResult_ShouldIndicateSuccess()
    {
        // Act
        var result = new ConversionResult
        {
            Success = true,
            Message = "File converted successfully",
            OutputFilePath = "converted_file.mp4",
            ProcessingTime = TimeSpan.FromSeconds(2.3)
        };

        // Assert
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Message.Should().NotBeEmpty();
        result.OutputFilePath.Should().NotBeEmpty();
        result.ProcessingTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void ConversionResult_FailureResult_ShouldIndicateFailure()
    {
        // Act
        var result = new ConversionResult
        {
            Success = false,
            Message = "Conversion failed",
            Exception = new FileNotFoundException("Input file not found")
        };

        // Assert
        result.Success.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<FileNotFoundException>();
        result.Message.Should().Contain("failed");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ConversionRequest_WithInvalidPaths_ShouldStillAcceptValues(string invalidPath)
    {
        // Act
        var request = new ConversionRequest
        {
            InputFilePath = invalidPath,
            OutputFilePath = invalidPath
        };

        // Assert - The model itself doesn't validate, validation happens in services
        if (invalidPath == null)
        {
            request.InputFilePath.Should().BeNull();
            request.OutputFilePath.Should().BeNull();
        }
        else
        {
            request.InputFilePath.Should().Be(invalidPath);
            request.OutputFilePath.Should().Be(invalidPath);
        }
    }

    [Theory]
    [InlineData(FileType.Unknown)]
    [InlineData(FileType.Jpeg)]
    [InlineData(FileType.Mp3)]
    [InlineData(FileType.Mp4)]
    public void ConversionRequest_WithAllFileTypes_ShouldAcceptValues(FileType fileType)
    {
        // Act
        var request = new ConversionRequest
        {
            TargetFormat = fileType
        };

        // Assert
        request.TargetFormat.Should().Be(fileType);
    }

    [Fact]
    public void ConversionResult_ProcessingTime_ShouldAcceptVariousTimeSpans()
    {
        // Arrange
        var timeSpans = new[]
        {
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromHours(1)
        };

        foreach (var timeSpan in timeSpans)
        {
            // Act
            var result = new ConversionResult
            {
                ProcessingTime = timeSpan
            };

            // Assert
            result.ProcessingTime.Should().Be(timeSpan);
        }
    }

    [Fact]
    public void ConversionRequest_Options_ShouldBeModifiableAfterInitialization()
    {
        // Arrange
        var request = new ConversionRequest();

        // Act
        request.Options.Add("Quality", 85);
        request.Options.Add("Bitrate", 192);
        request.Options["Quality"] = 95; // Modify existing

        // Assert
        request.Options.Should().HaveCount(2);
        request.Options["Quality"].Should().Be(95);
        request.Options["Bitrate"].Should().Be(192);
    }

    [Fact]
    public void ConversionResult_WithLongMessage_ShouldAcceptLongStrings()
    {
        // Arrange
        var longMessage = new string('A', 1000);

        // Act
        var result = new ConversionResult
        {
            Message = longMessage
        };

        // Assert
        result.Message.Should().Be(longMessage);
        result.Message.Length.Should().Be(1000);
    }

    [Fact]
    public void ConversionRequest_WithComplexOptions_ShouldHandleComplexObjects()
    {
        // Arrange & Act
        var complexOption = new { Width = 1920, Height = 1080, Quality = "High" };
        var request = new ConversionRequest
        {
            Options = new Dictionary<string, object>
            {
                { "VideoSettings", complexOption },
                { "List", new List<int> { 1, 2, 3 } },
                { "Dictionary", new Dictionary<string, string> { { "key", "value" } } }
            }
        };

        // Assert
        request.Options["VideoSettings"].Should().Be(complexOption);
        request.Options["List"].Should().BeAssignableTo<List<int>>();
        request.Options["Dictionary"].Should().BeAssignableTo<Dictionary<string, string>>();
    }
} 