using FileConverter.Core.Interfaces;
using FileConverter.Core.Services;
using FileConverter.WPF.ViewModels;
using FileConverter.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;

namespace FileConverter.WPF;

public partial class App : Application
{
    public static ServiceProvider? ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();

        // Initialize FFmpeg asynchronously
        Task.Run(async () =>
        {
            try
            {
                var success = await FFmpegService.EnsureFFmpegAsync();
                if (!success)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ShowFFmpegInstallDialog();
                    });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        $"Error initializing FFmpeg: {ex.Message}\n\n" +
                        "Audio and video conversions will not work without FFmpeg.",
                        "FFmpeg Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                });
            }
        });

        // Create and show the main window with proper dependency injection
        var mainWindow = new MainWindow();
        var viewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.DataContext = viewModel;
        mainWindow.Show();

        base.OnStartup(e);
    }

    private static void ShowFFmpegInstallDialog()
    {
        var result = MessageBox.Show(
            "FFmpeg is required for audio and video conversions but was not found.\n\n" +
            "Would you like to:\n" +
            "• Click 'Yes' to open the FFmpeg download page\n" +
            "• Click 'No' to continue without audio/video support\n" +
            "• Click 'Cancel' to see manual installation instructions",
            "FFmpeg Required",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Question);

        switch (result)
        {
            case MessageBoxResult.Yes:
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://ffmpeg.org/download.html",
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open browser: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                break;

            case MessageBoxResult.Cancel:
                MessageBox.Show(
                    FFmpegService.GetInstallationInstructions() + 
                    "\n\nRestart the application after installing FFmpeg.",
                    "FFmpeg Installation Instructions",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                break;

            case MessageBoxResult.No:
            default:
                // Continue without FFmpeg
                break;
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Register Core Services
        services.AddSingleton<IFileTypeDetector, FileTypeDetector>();
        services.AddSingleton<IConversionService, ConversionService>();

        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ConversionItemViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ServiceProvider?.Dispose();
        base.OnExit(e);
    }
} 