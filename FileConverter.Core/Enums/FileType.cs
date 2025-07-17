namespace FileConverter.Core.Enums;

public enum FileType
{
    Unknown,
    
    // Image formats
    Jpeg,
    Png,
    Bmp,
    Gif,
    Webp,
    Tiff,
    
    // Audio formats
    Mp3,
    Wav,
    Flac,
    Aac,
    Ogg,
    M4a,
    
    // Video formats
    Mp4,
    Avi,
    Mov,
    Mkv,
    Webm,
    Wmv,
    Flv
}

public enum ConversionCategory
{
    Image,
    Audio,
    Video
} 