# PowerShell script to run tests and save output
Write-Host "Building and running tests..." -ForegroundColor Green

try {
    # Build the test project
    Write-Host "Building FileConverter.Tests..." -ForegroundColor Yellow
    dotnet build FileConverter.Tests --verbosity minimal

    # Run the tests with detailed output
    Write-Host "Running tests..." -ForegroundColor Yellow
    dotnet test FileConverter.Tests --verbosity normal --logger "console;verbosity=detailed" 2>&1 | Tee-Object -FilePath "test-results.txt"
    
    Write-Host "Test results saved to test-results.txt" -ForegroundColor Green
}
catch {
    Write-Host "Error occurred: $_" -ForegroundColor Red
}

# Show summary
if (Test-Path "test-results.txt") {
    Write-Host "`nTest Summary:" -ForegroundColor Cyan
    Get-Content "test-results.txt" | Select-String "Test summary:" -A 1
} 