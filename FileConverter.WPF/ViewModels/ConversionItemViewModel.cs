using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;
using FileConverter.Core.Models;
using FileConverter.WPF.Models;
using System.IO;
using System.Windows;

namespace FileConverter.WPF.ViewModels;

public partial class ConversionItemViewModel : ObservableObject
{
    private readonly IConversionService _conversionService;

    [ObservableProperty]
    private string fileName = string.Empty;

    [ObservableProperty]
    private string filePath = string.Empty;

    [ObservableProperty]
    private long fileSize;

    [ObservableProperty]
    private FileType sourceFormat;

    [ObservableProperty]
    private FileType targetFormat;

    [ObservableProperty]
    private string outputPath = string.Empty;

    [ObservableProperty]
    private ConversionStatus status = ConversionStatus.Pending;

    [ObservableProperty]
    private double progress;

    [ObservableProperty]
    private string statusMessage = "Ready to convert";

    [ObservableProperty]
    private TimeSpan processingTime;

    [ObservableProperty]
    private bool isSelected;

    public ConversionItemViewModel(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    public ConversionItemViewModel(string filePath, FileType sourceFormat, IConversionService conversionService)
        : this(conversionService)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        SourceFormat = sourceFormat;
        
        if (File.Exists(filePath))
        {
            var fileInfo = new FileInfo(filePath);
            FileSize = fileInfo.Length;
        }

        // Set default target format based on source category
        var category = GetConversionCategory(sourceFormat);
        TargetFormat = GetDefaultTargetFormat(category);
        
        // Generate default output path
        GenerateOutputPath();
    }

    [RelayCommand]
    private async Task ConvertAsync()
    {
        if (Status == ConversionStatus.Converting)
            return;

        try
        {
            Status = ConversionStatus.Converting;
            StatusMessage = "Converting...";
            Progress = 0;

            var startTime = DateTime.Now;

            // Simulate progress updates (in real scenario, this would come from the conversion service)
            var progressTask = Task.Run(async () =>
            {
                for (int i = 0; i <= 90; i += 10)
                {
                    if (Status != ConversionStatus.Converting) break;
                    Progress = i;
                    await Task.Delay(100);
                }
            });

            var result = await _conversionService.ConvertFileAsync(FilePath, OutputPath, TargetFormat);

            ProcessingTime = DateTime.Now - startTime;
            Progress = 100;

            if (result.Success)
            {
                Status = ConversionStatus.Completed;
                StatusMessage = "Conversion completed successfully";
                OutputPath = result.OutputFilePath;
            }
            else
            {
                Status = ConversionStatus.Failed;
                StatusMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            Status = ConversionStatus.Failed;
            StatusMessage = $"Error: {ex.Message}";
            Progress = 0;
        }
    }

    [RelayCommand]
    private void OpenOutputFolder()
    {
        if (!string.IsNullOrEmpty(OutputPath) && File.Exists(OutputPath))
        {
            var folderPath = Path.GetDirectoryName(OutputPath);
            if (!string.IsNullOrEmpty(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{OutputPath}\"");
            }
        }
    }

    [RelayCommand]
    private void Remove()
    {
        // This will be handled by the parent ViewModel
        OnRemoveRequested?.Invoke(this);
    }

    public event Action<ConversionItemViewModel>? OnRemoveRequested;

    public void UpdateTargetFormat(FileType newFormat)
    {
        TargetFormat = newFormat;
        GenerateOutputPath();
    }

    private void GenerateOutputPath()
    {
        if (string.IsNullOrEmpty(FilePath))
            return;

        var directory = Path.GetDirectoryName(FilePath) ?? string.Empty;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
        var extension = GetFileExtension(TargetFormat);
        
        OutputPath = Path.Combine(directory, $"{fileNameWithoutExtension}_converted{extension}");
    }

    private static ConversionCategory GetConversionCategory(FileType fileType)
    {
        return fileType switch
        {
            FileType.Jpeg or FileType.Png or FileType.Bmp or FileType.Gif or FileType.Webp or FileType.Tiff => ConversionCategory.Image,
            FileType.Mp3 or FileType.Wav or FileType.Flac or FileType.Aac or FileType.Ogg or FileType.M4a => ConversionCategory.Audio,
            FileType.Mp4 or FileType.Avi or FileType.Mov or FileType.Mkv or FileType.Webm or FileType.Wmv or FileType.Flv => ConversionCategory.Video,
            _ => ConversionCategory.Image
        };
    }

    private static FileType GetDefaultTargetFormat(ConversionCategory category)
    {
        return category switch
        {
            ConversionCategory.Image => FileType.Png,
            ConversionCategory.Audio => FileType.Mp3,
            ConversionCategory.Video => FileType.Mp4,
            _ => FileType.Png
        };
    }

    private static string GetFileExtension(FileType fileType)
    {
        return fileType switch
        {
            FileType.Jpeg => ".jpg",
            FileType.Png => ".png",
            FileType.Bmp => ".bmp",
            FileType.Gif => ".gif",
            FileType.Webp => ".webp",
            FileType.Tiff => ".tiff",
            FileType.Mp3 => ".mp3",
            FileType.Wav => ".wav",
            FileType.Flac => ".flac",
            FileType.Aac => ".aac",
            FileType.Ogg => ".ogg",
            FileType.M4a => ".m4a",
            FileType.Mp4 => ".mp4",
            FileType.Avi => ".avi",
            FileType.Mov => ".mov",
            FileType.Mkv => ".mkv",
            FileType.Webm => ".webm",
            FileType.Wmv => ".wmv",
            FileType.Flv => ".flv",
            _ => ".unknown"
        };
    }
} 