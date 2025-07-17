using FileConverter.Core.Services;
using FileConverter.Core.Interfaces;
using FileConverter.Core.Enums;

// Example of using FileConverter programmatically

// Setup services
IFileTypeDetector detector = new FileTypeDetector();
IConversionService converter = new ConversionService(detector);

// Example 1: Convert image
var imageResult = await converter.ConvertFileAsync("input.png", "output.jpg");
if (imageResult.Success)
{
    Console.WriteLine($"Image conversion successful: {imageResult.Message}");
    Console.WriteLine($"Processing time: {imageResult.ProcessingTime.TotalSeconds:F2} seconds");
}
else
{
    Console.WriteLine($"Image conversion failed: {imageResult.Message}");
}

// Example 2: Convert audio
var audioResult = await converter.ConvertFileAsync("input.wav", "output.mp3");
if (audioResult.Success)
{
    Console.WriteLine($"Audio conversion successful: {audioResult.Message}");
    Console.WriteLine($"Processing time: {audioResult.ProcessingTime.TotalSeconds:F2} seconds");
}
else
{
    Console.WriteLine($"Audio conversion failed: {audioResult.Message}");
}

// Example 3: Convert video
var videoResult = await converter.ConvertFileAsync("input.avi", "output.mp4");
if (videoResult.Success)
{
    Console.WriteLine($"Video conversion successful: {videoResult.Message}");
    Console.WriteLine($"Processing time: {videoResult.ProcessingTime.TotalSeconds:F2} seconds");
}
else
{
    Console.WriteLine($"Video conversion failed: {videoResult.Message}");
}

// Example 4: Get supported formats
var imageFormats = converter.GetSupportedFormats(ConversionCategory.Image);
var audioFormats = converter.GetSupportedFormats(ConversionCategory.Audio);
var videoFormats = converter.GetSupportedFormats(ConversionCategory.Video);

Console.WriteLine($"Supported image formats: {string.Join(", ", imageFormats)}");
Console.WriteLine($"Supported audio formats: {string.Join(", ", audioFormats)}");
Console.WriteLine($"Supported video formats: {string.Join(", ", videoFormats)}"); 