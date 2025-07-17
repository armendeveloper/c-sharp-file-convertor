using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;
using FileConverter.Core.Models;

namespace FileConverter.Core.Services;

public class ConversionService : IConversionService
{
    private readonly IFileTypeDetector _fileTypeDetector;
    private readonly Dictionary<ConversionCategory, IFileConverter> _converters;

    public ConversionService(IFileTypeDetector fileTypeDetector)
    {
        _fileTypeDetector = fileTypeDetector ?? throw new ArgumentNullException(nameof(fileTypeDetector));
        _converters = new Dictionary<ConversionCategory, IFileConverter>
        {
            { ConversionCategory.Image, new ImageConverter() },
            { ConversionCategory.Audio, new AudioConverter() },
            { ConversionCategory.Video, new VideoConverter() }
        };
    }

    public async Task<ConversionResult> ConvertFileAsync(string inputPath, string outputPath, FileType? targetFormat = null)
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(inputPath) || string.IsNullOrEmpty(outputPath))
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "Invalid input or output path",
                    Exception = new ArgumentException("Input and output paths cannot be null or empty")
                };
            }

            if (!File.Exists(inputPath))
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "Input file does not exist.",
                    Exception = new FileNotFoundException($"Input file not found: {inputPath}")
                };
            }

            var sourceFormat = _fileTypeDetector.DetectFileType(inputPath);
            if (sourceFormat == FileType.Unknown)
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "Unsupported file type"
                };
            }

            FileType target;
            if (targetFormat.HasValue)
            {
                target = targetFormat.Value;
            }
            else
            {
                target = _fileTypeDetector.DetectFileType(outputPath);
                if (target == FileType.Unknown)
                {
                    return new ConversionResult
                    {
                        Success = false,
                        Message = "Unable to determine target file format from output path."
                    };
                }
            }

            var sourceCategory = _fileTypeDetector.GetCategory(sourceFormat);
            var targetCategory = _fileTypeDetector.GetCategory(target);

            if (sourceCategory != targetCategory)
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = $"Cannot convert between different media types: {sourceCategory} to {targetCategory}"
                };
            }

            if (!_converters.TryGetValue(sourceCategory, out var converter))
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = $"No converter available for category: {sourceCategory}"
                };
            }

            if (!converter.CanConvert(sourceFormat, target))
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = $"Conversion from {sourceFormat} to {target} is not supported."
                };
            }

            var request = new ConversionRequest
            {
                InputFilePath = inputPath,
                OutputFilePath = outputPath,
                TargetFormat = target
            };

            return await converter.ConvertAsync(request);
        }
        catch (Exception ex)
        {
            return new ConversionResult
            {
                Success = false,
                Message = $"Conversion failed: {ex.Message}",
                Exception = ex
            };
        }
    }

    public List<FileType> GetSupportedFormats(ConversionCategory category)
    {
        return category switch
        {
            ConversionCategory.Image => new List<FileType> 
            { 
                FileType.Jpeg, FileType.Png, FileType.Bmp, 
                FileType.Gif, FileType.Webp, FileType.Tiff 
            },
            ConversionCategory.Audio => new List<FileType> 
            { 
                FileType.Mp3, FileType.Wav, FileType.Flac, 
                FileType.Aac, FileType.Ogg, FileType.M4a 
            },
            ConversionCategory.Video => new List<FileType> 
            { 
                FileType.Mp4, FileType.Avi, FileType.Mov, 
                FileType.Mkv, FileType.Webm, FileType.Wmv, FileType.Flv 
            },
            _ => new List<FileType>()
        };
    }

    public bool IsFormatSupported(FileType format)
    {
        return format != FileType.Unknown && 
               GetSupportedFormats(ConversionCategory.Image).Contains(format) ||
               GetSupportedFormats(ConversionCategory.Audio).Contains(format) ||
               GetSupportedFormats(ConversionCategory.Video).Contains(format);
    }
} 