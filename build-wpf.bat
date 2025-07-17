@echo off
echo Building File Converter WPF Application...
echo.

echo Restoring NuGet packages...
dotnet restore FileConverter.WPF/FileConverter.WPF.csproj
if %errorlevel% neq 0 (
    echo Failed to restore packages.
    pause
    exit /b %errorlevel%
)

echo Building solution...
dotnet build FileConverter.WPF/FileConverter.WPF.csproj --configuration Release
if %errorlevel% neq 0 (
    echo Build failed.
    pause
    exit /b %errorlevel%
)

echo.
echo Build completed successfully!
echo.
echo To run the application, execute:
echo dotnet run --project FileConverter.WPF
echo.
echo Or navigate to the bin folder and run FileConverter.WPF.exe
echo.
pause 