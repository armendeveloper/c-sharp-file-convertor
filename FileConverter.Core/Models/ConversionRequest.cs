using FileConverter.Core.Enums;

namespace FileConverter.Core.Models;

public class ConversionRequest
{
    public string? InputFilePath { get; set; } = string.Empty;
    public string? OutputFilePath { get; set; } = string.Empty;
    public FileType TargetFormat { get; set; }
    public Dictionary<string, object> Options { get; set; } = new();
}

public class ConversionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string OutputFilePath { get; set; } = string.Empty;
    public TimeSpan ProcessingTime { get; set; }
    public Exception? Exception { get; set; }
} 