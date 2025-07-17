using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FileConverter.Core.Enums;
using FileConverter.Core.Interfaces;
using FileConverter.WPF.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace FileConverter.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IConversionService _conversionService;
    private readonly IFileTypeDetector _fileTypeDetector;

    [ObservableProperty]
    private ObservableCollection<ConversionItemViewModel> conversionItems = new();

    [ObservableProperty]
    private bool isConverting = false;

    [ObservableProperty]
    private double overallProgress = 0;

    [ObservableProperty]
    private string statusText = "Ready to convert files";

    [ObservableProperty]
    private int totalFiles = 0;

    [ObservableProperty]
    private int completedFiles = 0;

    [ObservableProperty]
    private int failedFiles = 0;

    [ObservableProperty]
    private bool hasFiles = false;

    [ObservableProperty]
    private ConversionCategory selectedCategory = ConversionCategory.Image;

    [ObservableProperty]
    private FileType selectedTargetFormat = FileType.Png;

    public List<ConversionCategory> Categories { get; } = new()
    {
        ConversionCategory.Image, ConversionCategory.Audio, ConversionCategory.Video
    };

    public List<FileType> AvailableImageFormats { get; } = new()
    {
        FileType.Jpeg, FileType.Png, FileType.Bmp, FileType.Gif, FileType.Webp, FileType.Tiff
    };

    public List<FileType> AvailableAudioFormats { get; } = new()
    {
        FileType.Mp3, FileType.Wav, FileType.Flac, FileType.Aac, FileType.Ogg, FileType.M4a
    };

    public List<FileType> AvailableVideoFormats { get; } = new()
    {
        FileType.Mp4, FileType.Avi, FileType.Mov, FileType.Mkv, FileType.Webm, FileType.Wmv, FileType.Flv
    };

    public MainWindowViewModel(IConversionService conversionService, IFileTypeDetector fileTypeDetector)
    {
        _conversionService = conversionService;
        _fileTypeDetector = fileTypeDetector;
        
        ConversionItems.CollectionChanged += (s, e) =>
        {
            HasFiles = ConversionItems.Count > 0;
            UpdateStatistics();
        };
    }

    [RelayCommand]
    private void SelectFiles()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select files to convert",
            Multiselect = true,
            Filter = "All Supported Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.tiff;*.mp3;*.wav;*.flac;*.aac;*.ogg;*.m4a;*.mp4;*.avi;*.mov;*.mkv;*.webm;*.wmv;*.flv|" +
                    "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.tiff|" +
                    "Audio Files|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.m4a|" +
                    "Video Files|*.mp4;*.avi;*.mov;*.mkv;*.webm;*.wmv;*.flv|" +
                    "All Files|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            foreach (var fileName in openFileDialog.FileNames)
            {
                AddFile(fileName);
            }
        }
    }

    [RelayCommand]
    private async Task ConvertAllAsync()
    {
        if (IsConverting || !HasFiles)
            return;

        IsConverting = true;
        StatusText = "Converting files...";
        
        var itemsToConvert = ConversionItems.Where(x => x.Status == ConversionStatus.Pending).ToList();
        TotalFiles = itemsToConvert.Count;
        CompletedFiles = 0;
        FailedFiles = 0;

        try
        {
            // Convert files with limited concurrency
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            var tasks = itemsToConvert.Select(async item =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await item.ConvertCommand.ExecuteAsync(null);
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (item.Status == ConversionStatus.Completed)
                            CompletedFiles++;
                        else if (item.Status == ConversionStatus.Failed)
                            FailedFiles++;
                        
                        UpdateProgress();
                    });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
        finally
        {
            IsConverting = false;
            StatusText = $"Conversion completed: {CompletedFiles} successful, {FailedFiles} failed";
        }
    }

    [RelayCommand]
    private void ClearCompleted()
    {
        var completedItems = ConversionItems.Where(x => x.Status == ConversionStatus.Completed).ToList();
        foreach (var item in completedItems)
        {
            ConversionItems.Remove(item);
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        ConversionItems.Clear();
    }

    [RelayCommand]
    private void ApplyTargetFormatToAll()
    {
        var itemsOfCategory = ConversionItems.Where(x => GetItemCategory(x.SourceFormat) == SelectedCategory).ToList();
        foreach (var item in itemsOfCategory)
        {
            item.UpdateTargetFormat(SelectedTargetFormat);
        }
    }

    public void HandleFileDrop(string[] files)
    {
        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                AddFile(file);
            }
            else if (Directory.Exists(file))
            {
                // Add all supported files from directory
                var supportedExtensions = new[]
                {
                    ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff",
                    ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a",
                    ".mp4", ".avi", ".mov", ".mkv", ".webm", ".wmv", ".flv"
                };

                var directoryFiles = Directory.GetFiles(file, "*.*", SearchOption.AllDirectories)
                    .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

                foreach (var directoryFile in directoryFiles)
                {
                    AddFile(directoryFile);
                }
            }
        }
    }

    private void AddFile(string filePath)
    {
        var fileType = _fileTypeDetector.DetectFileType(filePath);
        if (fileType == FileType.Unknown)
        {
            MessageBox.Show($"Unsupported file type: {Path.GetFileName(filePath)}", "Unsupported File", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Check if file already exists
        if (ConversionItems.Any(x => x.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var conversionItem = new ConversionItemViewModel(filePath, fileType, _conversionService);
        conversionItem.OnRemoveRequested += RemoveConversionItem;
        ConversionItems.Add(conversionItem);
    }

    private void RemoveConversionItem(ConversionItemViewModel item)
    {
        item.OnRemoveRequested -= RemoveConversionItem;
        ConversionItems.Remove(item);
    }

    private void UpdateProgress()
    {
        if (TotalFiles == 0)
        {
            OverallProgress = 0;
            return;
        }

        var progressSum = ConversionItems.Sum(x => x.Progress);
        OverallProgress = progressSum / ConversionItems.Count;
    }

    private void UpdateStatistics()
    {
        TotalFiles = ConversionItems.Count;
        CompletedFiles = ConversionItems.Count(x => x.Status == ConversionStatus.Completed);
        FailedFiles = ConversionItems.Count(x => x.Status == ConversionStatus.Failed);
    }

    private static ConversionCategory GetItemCategory(FileType fileType)
    {
        return fileType switch
        {
            FileType.Jpeg or FileType.Png or FileType.Bmp or FileType.Gif or FileType.Webp or FileType.Tiff => ConversionCategory.Image,
            FileType.Mp3 or FileType.Wav or FileType.Flac or FileType.Aac or FileType.Ogg or FileType.M4a => ConversionCategory.Audio,
            FileType.Mp4 or FileType.Avi or FileType.Mov or FileType.Mkv or FileType.Webm or FileType.Wmv or FileType.Flv => ConversionCategory.Video,
            _ => ConversionCategory.Image
        };
    }

    partial void OnSelectedCategoryChanged(ConversionCategory value)
    {
        SelectedTargetFormat = value switch
        {
            ConversionCategory.Image => FileType.Png,
            ConversionCategory.Audio => FileType.Mp3,
            ConversionCategory.Video => FileType.Mp4,
            _ => FileType.Png
        };
    }
} 