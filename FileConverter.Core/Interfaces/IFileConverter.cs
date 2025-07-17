using FileConverter.Core.Enums;
using FileConverter.Core.Models;

namespace FileConverter.Core.Interfaces;

public interface IFileConverter
{
    Task<ConversionResult> ConvertAsync(ConversionRequest request);
    bool CanConvert(FileType sourceFormat, FileType targetFormat);
    ConversionCategory SupportedCategory { get; }
}

public interface IFileTypeDetector
{
    FileType DetectFileType(string filePath);
    ConversionCategory GetCategory(FileType fileType);
}

public interface IConversionService
{
    Task<ConversionResult> ConvertFileAsync(string inputPath, string outputPath, FileType? targetFormat = null);
    List<FileType> GetSupportedFormats(ConversionCategory category);
    bool IsFormatSupported(FileType format);
} 