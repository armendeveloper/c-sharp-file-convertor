using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;

namespace FileConverter.Core.Services;

public class FileTypeDetector : IFileTypeDetector
{
    private static readonly Dictionary<string, FileType> ExtensionMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        // Image formats
        { ".jpg", FileType.Jpeg },
        { ".jpeg", FileType.Jpeg },
        { ".png", FileType.Png },
        { ".bmp", FileType.Bmp },
        { ".gif", FileType.Gif },
        { ".webp", FileType.Webp },
        { ".tiff", FileType.Tiff },
        { ".tif", FileType.Tiff },
        
        // Audio formats
        { ".mp3", FileType.Mp3 },
        { ".wav", FileType.Wav },
        { ".flac", FileType.Flac },
        { ".aac", FileType.Aac },
        { ".ogg", FileType.Ogg },
        { ".m4a", FileType.M4a },
        
        // Video formats
        { ".mp4", FileType.Mp4 },
        { ".avi", FileType.Avi },
        { ".mov", FileType.Mov },
        { ".mkv", FileType.Mkv },
        { ".webm", FileType.Webm },
        { ".wmv", FileType.Wmv },
        { ".flv", FileType.Flv }
    };

    private static readonly Dictionary<FileType, ConversionCategory> CategoryMapping = new()
    {
        // Images
        { FileType.Jpeg, ConversionCategory.Image },
        { FileType.Png, ConversionCategory.Image },
        { FileType.Bmp, ConversionCategory.Image },
        { FileType.Gif, ConversionCategory.Image },
        { FileType.Webp, ConversionCategory.Image },
        { FileType.Tiff, ConversionCategory.Image },
        
        // Audio
        { FileType.Mp3, ConversionCategory.Audio },
        { FileType.Wav, ConversionCategory.Audio },
        { FileType.Flac, ConversionCategory.Audio },
        { FileType.Aac, ConversionCategory.Audio },
        { FileType.Ogg, ConversionCategory.Audio },
        { FileType.M4a, ConversionCategory.Audio },
        
        // Video
        { FileType.Mp4, ConversionCategory.Video },
        { FileType.Avi, ConversionCategory.Video },
        { FileType.Mov, ConversionCategory.Video },
        { FileType.Mkv, ConversionCategory.Video },
        { FileType.Webm, ConversionCategory.Video },
        { FileType.Wmv, ConversionCategory.Video },
        { FileType.Flv, ConversionCategory.Video }
    };

    public FileType DetectFileType(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return FileType.Unknown;

        var extension = Path.GetExtension(filePath);
        return ExtensionMapping.TryGetValue(extension, out var fileType) ? fileType : FileType.Unknown;
    }

    public ConversionCategory GetCategory(FileType fileType)
    {
        return CategoryMapping.TryGetValue(fileType, out var category) ? category : ConversionCategory.Image;
    }
} 