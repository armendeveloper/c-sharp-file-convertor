using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;
using FileConverter.Core.Models;
using FFMpegCore;
using FFMpegCore.Enums;
using System.Diagnostics;

namespace FileConverter.Core.Services;

public class AudioConverter : IFileConverter
{
    public ConversionCategory SupportedCategory => ConversionCategory.Audio;

    private static readonly HashSet<FileType> SupportedFormats = new()
    {
        FileType.Mp3, FileType.Wav, FileType.Flac, 
        FileType.Aac, FileType.Ogg, FileType.M4a
    };

    private static readonly Dictionary<FileType, string> FormatExtensions = new()
    {
        { FileType.Mp3, ".mp3" },
        { FileType.Wav, ".wav" },
        { FileType.Flac, ".flac" },
        { FileType.Aac, ".aac" },
        { FileType.Ogg, ".ogg" },
        { FileType.M4a, ".m4a" }
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
                    Message = "Unsupported target format for audio conversion",
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
                .OutputToFile(request.OutputFilePath, true, options => ConfigureAudioOptions(options, request.TargetFormat, request.Options));

            var result = await conversion.ProcessAsynchronously();

            stopwatch.Stop();

            if (result)
            {
                return new ConversionResult
                {
                    Success = true,
                    Message = $"Successfully converted audio to {request.TargetFormat}",
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
                Message = $"Failed to convert audio: {ex.Message}",
                Exception = ex,
                ProcessingTime = stopwatch.Elapsed
            };
        }
    }

    private static void ConfigureAudioOptions(FFMpegArgumentOptions options, FileType targetFormat, Dictionary<string, object> conversionOptions)
    {
        var bitrate = GetIntOption(conversionOptions, "bitrate", 128);
        var sampleRate = GetIntOption(conversionOptions, "sampleRate", 44100);

        options = targetFormat switch
        {
            FileType.Mp3 => options.WithAudioCodec("libmp3lame").WithAudioBitrate(bitrate),
            FileType.Wav => options.WithAudioCodec("pcm_s16le").WithAudioSamplingRate(sampleRate),
            FileType.Flac => options.WithAudioCodec("flac"),
            FileType.Aac => options.WithAudioCodec("aac").WithAudioBitrate(bitrate),
            FileType.Ogg => options.WithAudioCodec("libvorbis").WithAudioBitrate(bitrate),
            FileType.M4a => options.WithAudioCodec("aac").WithAudioBitrate(bitrate),
            _ => options
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