using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;
using FileConverter.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using System.Diagnostics;

namespace FileConverter.Core.Services;

public class ImageConverter : IFileConverter
{
    public ConversionCategory SupportedCategory => ConversionCategory.Image;

    private static readonly HashSet<FileType> SupportedFormats = new()
    {
        FileType.Jpeg, FileType.Png, FileType.Bmp, 
        FileType.Gif, FileType.Webp, FileType.Tiff
    };

    public bool CanConvert(FileType sourceFormat, FileType targetFormat)
    {
        return SupportedFormats.Contains(sourceFormat) && SupportedFormats.Contains(targetFormat);
    }

    public async Task<ConversionResult> ConvertAsync(ConversionRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            if (request == null)
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "Request cannot be null",
                    Exception = new ArgumentNullException(nameof(request)),
                    ProcessingTime = stopwatch.Elapsed
                };
            }

            if (string.IsNullOrEmpty(request.InputFilePath) || string.IsNullOrEmpty(request.OutputFilePath))
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "Invalid input or output path",
                    ProcessingTime = stopwatch.Elapsed
                };
            }

            if (!SupportedFormats.Contains(request.TargetFormat))
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "Unsupported target format for image conversion",
                    ProcessingTime = stopwatch.Elapsed
                };
            }

            if (!File.Exists(request.InputFilePath))
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "Input file does not exist.",
                    ProcessingTime = stopwatch.Elapsed
                };
            }

            var outputDir = Path.GetDirectoryName(request.OutputFilePath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            using var image = await Image.LoadAsync(request.InputFilePath);
            
            var encoder = GetEncoder(request.TargetFormat, request.Options);
            await image.SaveAsync(request.OutputFilePath, encoder);

            stopwatch.Stop();

            return new ConversionResult
            {
                Success = true,
                Message = $"Successfully converted image to {request.TargetFormat}",
                OutputFilePath = request.OutputFilePath,
                ProcessingTime = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ConversionResult
            {
                Success = false,
                Message = $"Failed to convert image: {ex.Message}",
                Exception = ex,
                ProcessingTime = stopwatch.Elapsed
            };
        }
    }

    private static IImageEncoder GetEncoder(FileType targetFormat, Dictionary<string, object> options)
    {
        return targetFormat switch
        {
            FileType.Jpeg => new JpegEncoder { Quality = GetIntOption(options, "quality", 75) },
            FileType.Png => new PngEncoder(),
            FileType.Bmp => new BmpEncoder(),
            FileType.Gif => new GifEncoder(),
            FileType.Webp => new WebpEncoder { Quality = GetIntOption(options, "quality", 75) },
            FileType.Tiff => new TiffEncoder(),
            _ => throw new ArgumentException($"Unsupported target format: {targetFormat}")
        };
    }

    private static int GetIntOption(Dictionary<string, object> options, string key, int defaultValue)
    {
        if (options.TryGetValue(key, out var value) && value is int intValue)
        {
            return intValue;
        }
        return defaultValue;
    }
} 