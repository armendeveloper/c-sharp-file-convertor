using FFMpegCore;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace FileConverter.Core.Services;

public static class FFmpegService
{
    private static readonly string FFmpegDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg");
    private static readonly string FFmpegExecutable = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
        ? Path.Combine(FFmpegDirectory, "ffmpeg.exe") 
        : Path.Combine(FFmpegDirectory, "ffmpeg");

    public static async Task<bool> EnsureFFmpegAsync()
    {
        try
        {
            // Check if FFmpeg is already available in system PATH
            if (IsFFmpegInSystemPath())
            {
                return true;
            }

            // Check if we have FFmpeg in our local directory
            if (File.Exists(FFmpegExecutable))
            {
                GlobalFFOptions.Configure(new FFOptions { BinaryFolder = FFmpegDirectory });
                return true;
            }

            // Try to download and extract FFmpeg
            var success = await DownloadFFmpegAsync();
            if (success)
            {
                GlobalFFOptions.Configure(new FFOptions { BinaryFolder = FFmpegDirectory });
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting up FFmpeg: {ex.Message}");
            return false;
        }
    }

    private static bool IsFFmpegInSystemPath()
    {
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = "-version";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            
            process.Start();
            process.WaitForExit(5000); // 5 second timeout
            
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> DownloadFFmpegAsync()
    {
        try
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // For non-Windows platforms, user needs to install FFmpeg manually
                return false;
            }

            // Create FFmpeg directory
            Directory.CreateDirectory(FFmpegDirectory);

            // Use a more reliable source - GitHub releases
            string downloadUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip";
            string zipPath = Path.Combine(FFmpegDirectory, "ffmpeg.zip");

            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "FileConverter/1.0");
            
            Console.WriteLine("Downloading FFmpeg... This may take a few minutes.");
            
            // Download with progress
            var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            
            if (!response.IsSuccessStatusCode)
            {
                // Try alternative source
                downloadUrl = "https://www.gyan.dev/ffmpeg/builds/packages/ffmpeg-6.1.1-essentials_build.zip";
                response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
            }

            await using var fileStream = File.Create(zipPath);
            await response.Content.CopyToAsync(fileStream);
            
            // Extract the zip file
            using var archive = ZipFile.OpenRead(zipPath);
            
            // Look for ffmpeg.exe in any subdirectory
            var ffmpegEntry = archive.Entries.FirstOrDefault(e => 
                e.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase) && 
                e.FullName.Contains("bin"));
            
            if (ffmpegEntry == null)
            {
                // Try without bin requirement
                ffmpegEntry = archive.Entries.FirstOrDefault(e => 
                    e.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase));
            }
            
            if (ffmpegEntry != null)
            {
                ffmpegEntry.ExtractToFile(FFmpegExecutable, overwrite: true);
            }

            // Also extract ffprobe.exe if available
            var ffprobeEntry = archive.Entries.FirstOrDefault(e => 
                e.Name.Equals("ffprobe.exe", StringComparison.OrdinalIgnoreCase));
            
            if (ffprobeEntry != null)
            {
                string ffprobePath = Path.Combine(FFmpegDirectory, "ffprobe.exe");
                ffprobeEntry.ExtractToFile(ffprobePath, overwrite: true);
            }

            // Clean up zip file
            File.Delete(zipPath);

            return File.Exists(FFmpegExecutable);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download FFmpeg: {ex.Message}");
            return false;
        }
    }

    public static string GetFFmpegPath()
    {
        return FFmpegExecutable;
    }

    public static bool IsAvailable()
    {
        return IsFFmpegInSystemPath() || File.Exists(FFmpegExecutable);
    }

    public static string GetInstallationInstructions()
    {
        return @"To manually install FFmpeg:

1. Download FFmpeg from: https://ffmpeg.org/download.html
2. Extract the files to a folder (e.g., C:\ffmpeg)
3. Add the bin folder to your system PATH
4. Or place ffmpeg.exe in the application folder

Alternative: Use Chocolatey or Winget
- choco install ffmpeg
- winget install FFmpeg";
    }
} 