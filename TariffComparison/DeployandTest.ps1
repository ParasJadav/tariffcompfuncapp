# Restore dependencies
dotnet restore

#change directory to Test project
cd..
cd .\TariffComparison.Test\

# Run unit tests
dotnet test --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Tests failed. Aborting publish."
    exit 1
}

#change directory to Main project
cd..
cd .\TariffComparison\
# Publish Project to Function App
Write-Host "✅ Tests passed. Publishing..."
func azure functionapp publish TariffComparisonApp
