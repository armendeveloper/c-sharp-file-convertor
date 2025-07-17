using FileConverter.Core.Enums;
using FluentAssertions;
using Xunit;

namespace FileConverter.Tests.Enums;

public class EnumTests
{
    [Fact]
    public void FileType_ShouldHaveAllExpectedImageFormats()
    {
        // Arrange
        var expectedImageFormats = new[]
        {
            FileType.Jpeg,
            FileType.Png,
            FileType.Bmp,
            FileType.Gif,
            FileType.Webp,
            FileType.Tiff
        };

        // Assert
        foreach (var format in expectedImageFormats)
        {
            Enum.IsDefined(typeof(FileType), format).Should().BeTrue($"FileType should define {format}");
        }
    }

    [Fact]
    public void FileType_ShouldHaveAllExpectedAudioFormats()
    {
        // Arrange
        var expectedAudioFormats = new[]
        {
            FileType.Mp3,
            FileType.Wav,
            FileType.Flac,
            FileType.Aac,
            FileType.Ogg,
            FileType.M4a
        };

        // Assert
        foreach (var format in expectedAudioFormats)
        {
            Enum.IsDefined(typeof(FileType), format).Should().BeTrue($"FileType should define {format}");
        }
    }

    [Fact]
    public void FileType_ShouldHaveAllExpectedVideoFormats()
    {
        // Arrange
        var expectedVideoFormats = new[]
        {
            FileType.Mp4,
            FileType.Avi,
            FileType.Mov,
            FileType.Mkv,
            FileType.Webm,
            FileType.Wmv,
            FileType.Flv
        };

        // Assert
        foreach (var format in expectedVideoFormats)
        {
            Enum.IsDefined(typeof(FileType), format).Should().BeTrue($"FileType should define {format}");
        }
    }

    [Fact]
    public void FileType_ShouldHaveUnknownValue()
    {
        // Assert
        Enum.IsDefined(typeof(FileType), FileType.Unknown).Should().BeTrue();
        ((int)FileType.Unknown).Should().Be(0, "Unknown should be the default value");
    }

    [Fact]
    public void ConversionCategory_ShouldHaveAllExpectedValues()
    {
        // Arrange
        var expectedCategories = new[]
        {
            ConversionCategory.Image,
            ConversionCategory.Audio,
            ConversionCategory.Video
        };

        // Assert
        foreach (var category in expectedCategories)
        {
            Enum.IsDefined(typeof(ConversionCategory), category).Should().BeTrue($"ConversionCategory should define {category}");
        }
    }

    [Fact]
    public void FileType_AllValues_ShouldBeUnique()
    {
        // Arrange
        var allValues = Enum.GetValues<FileType>();

        // Assert
        allValues.Should().OnlyHaveUniqueItems("All FileType values should be unique");
    }

    [Fact]
    public void ConversionCategory_AllValues_ShouldBeUnique()
    {
        // Arrange
        var allValues = Enum.GetValues<ConversionCategory>();

        // Assert
        allValues.Should().OnlyHaveUniqueItems("All ConversionCategory values should be unique");
    }

    [Theory]
    [InlineData(FileType.Jpeg)]
    [InlineData(FileType.Png)]
    [InlineData(FileType.Mp3)]
    [InlineData(FileType.Mp4)]
    [InlineData(FileType.Unknown)]
    public void FileType_ToString_ShouldReturnValidString(FileType fileType)
    {
        // Act
        var result = fileType.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotContain(" ", "Enum string representation should not contain spaces");
    }

    [Theory]
    [InlineData(ConversionCategory.Image)]
    [InlineData(ConversionCategory.Audio)]
    [InlineData(ConversionCategory.Video)]
    public void ConversionCategory_ToString_ShouldReturnValidString(ConversionCategory category)
    {
        // Act
        var result = category.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotContain(" ", "Enum string representation should not contain spaces");
    }

    [Fact]
    public void FileType_ShouldHaveExpectedTotalCount()
    {
        // Arrange
        var allFileTypes = Enum.GetValues<FileType>();
        
        // Assert - 6 image + 6 audio + 7 video + 1 unknown = 20
        allFileTypes.Should().HaveCount(20, "Should have 19 supported formats plus Unknown");
    }

    [Fact]
    public void ConversionCategory_ShouldHaveExpectedTotalCount()
    {
        // Arrange
        var allCategories = Enum.GetValues<ConversionCategory>();
        
        // Assert
        allCategories.Should().HaveCount(3, "Should have exactly 3 conversion categories");
    }

    [Fact]
    public void FileType_EnumValues_ShouldBeContiguous()
    {
        // Arrange
        var values = Enum.GetValues<FileType>().Cast<int>().OrderBy(x => x).ToArray();
        
        // Assert - Values should start from 0 and be contiguous
        for (int i = 0; i < values.Length; i++)
        {
            values[i].Should().Be(i, $"Enum value at index {i} should be {i} for contiguous numbering");
        }
    }

    [Fact]
    public void ConversionCategory_EnumValues_ShouldBeContiguous()
    {
        // Arrange
        var values = Enum.GetValues<ConversionCategory>().Cast<int>().OrderBy(x => x).ToArray();
        
        // Assert - Values should start from 0 and be contiguous
        for (int i = 0; i < values.Length; i++)
        {
            values[i].Should().Be(i, $"Enum value at index {i} should be {i} for contiguous numbering");
        }
    }

    [Theory]
    [InlineData("Jpeg")]
    [InlineData("Mp3")]
    [InlineData("Mp4")]
    public void FileType_ParseFromString_ShouldWork(string enumName)
    {
        // Act
        var parsed = Enum.Parse<FileType>(enumName);
        
        // Assert
        parsed.ToString().Should().Be(enumName);
        Enum.IsDefined(typeof(FileType), parsed).Should().BeTrue();
    }

    [Theory]
    [InlineData("Image")]
    [InlineData("Audio")]
    [InlineData("Video")]
    public void ConversionCategory_ParseFromString_ShouldWork(string enumName)
    {
        // Act
        var parsed = Enum.Parse<ConversionCategory>(enumName);
        
        // Assert
        parsed.ToString().Should().Be(enumName);
        Enum.IsDefined(typeof(ConversionCategory), parsed).Should().BeTrue();
    }

    [Fact]
    public void FileType_InvalidParse_ShouldThrowException()
    {
        // Act & Assert
        var action = () => Enum.Parse<FileType>("InvalidFormat");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ConversionCategory_InvalidParse_ShouldThrowException()
    {
        // Act & Assert
        var action = () => Enum.Parse<ConversionCategory>("InvalidCategory");
        action.Should().Throw<ArgumentException>();
    }
} 