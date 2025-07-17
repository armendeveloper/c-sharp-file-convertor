# FileConverter Test Script
# This script demonstrates the file conversion capabilities

Write-Host "FileConverter Test Script" -ForegroundColor Green
Write-Host "========================" -ForegroundColor Green

# Test 1: Show supported formats
Write-Host "`n1. Testing supported formats:" -ForegroundColor Yellow
dotnet run --project FileConverter.CLI -- --formats

# Test 2: Show help
Write-Host "`n2. Testing help:" -ForegroundColor Yellow
dotnet run --project FileConverter.CLI -- --help

Write-Host "`n3. Manual testing examples:" -ForegroundColor Yellow
Write-Host "To test actual file conversion, you would run:" -ForegroundColor Cyan
Write-Host "  dotnet run --project FileConverter.CLI -- input.png output.jpg" -ForegroundColor White
Write-Host "  dotnet run --project FileConverter.CLI -- -i audio.wav -o audio.mp3" -ForegroundColor White
Write-Host "  dotnet run --project FileConverter.CLI -- video.avi video.mp4" -ForegroundColor White

Write-Host "`nNote: For audio/video conversion, FFmpeg must be installed on your system." -ForegroundColor Red
Write-Host "Image conversion works without FFmpeg using ImageSharp library." -ForegroundColor Green 