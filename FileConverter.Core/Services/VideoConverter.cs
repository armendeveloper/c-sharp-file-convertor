using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;
using FileConverter.Core.Models;
using FFMpegCore;
using FFMpegCore.Enums;
using System.Diagnostics;

namespace FileConverter.Core.Services;

public class VideoConverter : IFileConverter
{
    public ConversionCategory SupportedCategory => ConversionCategory.Video;

    private static readonly HashSet<FileType> SupportedFormats = new()
    {
        FileType.Mp4, FileType.Avi, FileType.Mov, 
        FileType.Mkv, FileType.Webm, FileType.Wmv, FileType.Flv
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
                    Message = "Unsupported target format for video conversion",
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

            // Check FFmpeg availability
            if (!FFmpegService.IsAvailable())
            {
                var ffmpegSetup = await FFmpegService.EnsureFFmpegAsync();
                if (!ffmpegSetup)
                {
                    return new ConversionResult
                    {
                        Success = false,
                        Message = "FFmpeg is not available. Please install FFmpeg or try again to download it automatically.",
                        ProcessingTime = stopwatch.Elapsed
                    };
                }
            }

            var outputDir = Path.GetDirectoryName(request.OutputFilePath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var conversion = FFMpegArguments
                .FromFileInput(request.InputFilePath)
                .OutputToFile(request.OutputFilePath, true, options => ConfigureVideoOptions(options, request.TargetFormat, request.Options));

            var result = await conversion.ProcessAsynchronously();

            stopwatch.Stop();

            if (result)
            {
                return new ConversionResult
                {
                    Success = true,
                    Message = $"Successfully converted video to {request.TargetFormat}",
                    OutputFilePath = request.OutputFilePath,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
            else
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = "FFmpeg conversion failed",
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new ConversionResult
            {
                Success = false,
                Message = $"Failed to convert video: {ex.Message}",
                Exception = ex,
                ProcessingTime = stopwatch.Elapsed
            };
        }
    }

    private static void ConfigureVideoOptions(FFMpegArgumentOptions options, FileType targetFormat, Dictionary<string, object> conversionOptions)
    {
        var videoBitrate = GetIntOption(conversionOptions, "videoBitrate", 1000);
        var audioBitrate = GetIntOption(conversionOptions, "audioBitrate", 128);
        var fps = GetIntOption(conversionOptions, "fps", 30);
        var width = GetIntOption(conversionOptions, "width", -1);
        var height = GetIntOption(conversionOptions, "height", -1);

        options = targetFormat switch
        {
            FileType.Mp4 => options.WithVideoCodec("libx264").WithAudioCodec("aac"),
            FileType.Avi => options.WithVideoCodec("libx264").WithAudioCodec("libmp3lame"),
            FileType.Mov => options.WithVideoCodec("libx264").WithAudioCodec("aac"),
            FileType.Mkv => options.WithVideoCodec("libx264").WithAudioCodec("aac"),
            FileType.Webm => options.WithVideoCodec("libvpx").WithAudioCodec("libvorbis"),
            FileType.Wmv => options.WithVideoCodec("libx264").WithAudioCodec("aac"),
            FileType.Flv => options.WithVideoCodec("libx264").WithAudioCodec("aac"),
            _ => options
        };

        options = options.WithVideoBitrate(videoBitrate).WithAudioBitrate(audioBitrate);

        if (fps > 0)
        {
            options = options.WithFramerate(fps);
        }

        if (width > 0 && height > 0)
        {
            options = options.Resize(width, height);
        }
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