using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;
using FileConverter.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileConverter.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return 0;
        }

        var host = CreateHostBuilder().Build();
        var conversionService = host.Services.GetRequiredService<IConversionService>();

        try
        {
            var options = ParseArguments(args);
            if (options == null)
            {
                ShowHelp();
                return 1;
            }

            if (options.ShowFormats)
            {
                ShowSupportedFormats(conversionService);
                return 0;
            }

            if (string.IsNullOrEmpty(options.InputPath) || string.IsNullOrEmpty(options.OutputPath))
            {
                Console.WriteLine("Error: Input and output paths are required.");
                ShowHelp();
                return 1;
            }

            Console.WriteLine($"Converting '{options.InputPath}' to '{options.OutputPath}'...");
            
            var result = await conversionService.ConvertFileAsync(
                options.InputPath, 
                options.OutputPath, 
                options.TargetFormat);

            if (result.Success)
            {
                Console.WriteLine($"✓ {result.Message}");
                Console.WriteLine($"  Processing time: {result.ProcessingTime.TotalSeconds:F2} seconds");
                Console.WriteLine($"  Output file: {result.OutputFilePath}");
                return 0;
            }
            else
            {
                Console.WriteLine($"✗ Conversion failed: {result.Message}");
                if (result.Exception != null)
                {
                    Console.WriteLine($"  Error details: {result.Exception.Message}");
                }
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Application error: {ex.Message}");
            return 1;
        }
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IFileTypeDetector, FileTypeDetector>();
                services.AddSingleton<IConversionService, ConversionService>();
            });
    }

    private static ConversionOptions? ParseArguments(string[] args)
    {
        var options = new ConversionOptions();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-i":
                case "--input":
                    if (i + 1 < args.Length)
                        options.InputPath = args[++i];
                    break;

                case "-o":
                case "--output":
                    if (i + 1 < args.Length)
                        options.OutputPath = args[++i];
                    break;

                case "-f":
                case "--format":
                    if (i + 1 < args.Length)
                    {
                        var formatString = args[++i];
                        if (Enum.TryParse<FileType>(formatString, true, out var format))
                        {
                            options.TargetFormat = format;
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Unknown format '{formatString}'. Will try to detect from output file extension.");
                        }
                    }
                    break;

                case "--formats":
                    options.ShowFormats = true;
                    break;

                case "-h":
                case "--help":
                    return null;

                default:
                    // If it doesn't start with -, treat as input path if not set
                    if (!args[i].StartsWith('-') && string.IsNullOrEmpty(options.InputPath))
                    {
                        options.InputPath = args[i];
                    }
                    // If input is set and this doesn't start with -, treat as output path
                    else if (!args[i].StartsWith('-') && string.IsNullOrEmpty(options.OutputPath))
                    {
                        options.OutputPath = args[i];
                    }
                    break;
            }
        }

        return options;
    }

    private static void ShowHelp()
    {
        Console.WriteLine("FileConverter - Convert between different file formats");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  FileConverter [options] <input> <output>");
        Console.WriteLine("  FileConverter -i <input> -o <output> [-f <format>]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -i, --input <path>     Input file path");
        Console.WriteLine("  -o, --output <path>    Output file path");
        Console.WriteLine("  -f, --format <format>  Target format (optional, auto-detected from output extension)");
        Console.WriteLine("  --formats              Show supported formats");
        Console.WriteLine("  -h, --help             Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  FileConverter image.png image.jpg");
        Console.WriteLine("  FileConverter -i audio.wav -o audio.mp3");
        Console.WriteLine("  FileConverter video.avi video.mp4 -f Mp4");
        Console.WriteLine("  FileConverter --formats");
        Console.WriteLine();
        Console.WriteLine("Supported categories: Images, Audio, Video");
        Console.WriteLine("Note: FFmpeg must be installed for audio/video conversion");
    }

    private static void ShowSupportedFormats(IConversionService conversionService)
    {
        Console.WriteLine("Supported File Formats:");
        Console.WriteLine();

        Console.WriteLine("Images:");
        var imageFormats = conversionService.GetSupportedFormats(ConversionCategory.Image);
        foreach (var format in imageFormats)
        {
            Console.WriteLine($"  - {format}");
        }

        Console.WriteLine();
        Console.WriteLine("Audio:");
        var audioFormats = conversionService.GetSupportedFormats(ConversionCategory.Audio);
        foreach (var format in audioFormats)
        {
            Console.WriteLine($"  - {format}");
        }

        Console.WriteLine();
        Console.WriteLine("Video:");
        var videoFormats = conversionService.GetSupportedFormats(ConversionCategory.Video);
        foreach (var format in videoFormats)
        {
            Console.WriteLine($"  - {format}");
        }
    }
}

public class ConversionOptions
{
    public string InputPath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public FileType? TargetFormat { get; set; }
    public bool ShowFormats { get; set; }
}
