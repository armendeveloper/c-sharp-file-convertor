@echo off
echo Building and running tests...
echo.

echo Building FileConverter.Tests...
dotnet build FileConverter.Tests --verbosity minimal

echo.
echo Running tests...
dotnet test FileConverter.Tests --verbosity normal > test-results.txt 2>&1

echo.
echo Test results saved to test-results.txt
echo.

if exist test-results.txt (
    echo Test Summary:
    findstr /C:"Test summary:" test-results.txt
    findstr /C:"failed:" test-results.txt
    findstr /C:"succeeded:" test-results.txt
)

pause 