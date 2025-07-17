@echo off
echo Installing FFmpeg for File Converter...
echo.

REM Try winget first (Windows 10/11)
echo Attempting to install FFmpeg using winget...
winget install FFmpeg.FFmpeg
if %errorlevel% equ 0 (
    echo.
    echo FFmpeg installed successfully via winget!
    echo Restart the File Converter application to use audio/video features.
    pause
    exit /b 0
)

echo.
echo Winget installation failed. Trying Chocolatey...

REM Try chocolatey
where choco >nul 2>nul
if %errorlevel% neq 0 (
    echo.
    echo Chocolatey not found. Installing Chocolatey first...
    powershell -Command "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
)

echo Installing FFmpeg via Chocolatey...
choco install ffmpeg -y
if %errorlevel% equ 0 (
    echo.
    echo FFmpeg installed successfully via Chocolatey!
    echo Restart the File Converter application to use audio/video features.
    pause
    exit /b 0
)

echo.
echo Automatic installation failed. Please install FFmpeg manually:
echo.
echo 1. Go to https://ffmpeg.org/download.html
echo 2. Download FFmpeg for Windows
echo 3. Extract to C:\ffmpeg
echo 4. Add C:\ffmpeg\bin to your system PATH
echo.
echo Alternative: Download portable version and place ffmpeg.exe in the application folder.
echo.
pause 