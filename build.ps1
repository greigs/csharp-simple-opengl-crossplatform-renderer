# This script builds and publishes the Renderer for Windows, macOS (Intel), and macOS (Apple Silicon).

Write-Host "Starting build process..."

# Define project path for clarity
$projectPath = "Renderer/Renderer.csproj"

# --- Build for Windows (win-x64) ---
Write-Host "Building for Windows (win-x64)..."
# Set OutputType to WinExe for a windowed application without a console
(Get-Content -Path $projectPath) -replace '<OutputType>Exe</OutputType>', '<OutputType>WinExe</OutputType>' | Set-Content -Path $projectPath
dotnet publish $projectPath -r win-x64 -c Release --self-contained true /p:PublishSingleFile=true

# --- Build for macOS (osx-x64) ---
Write-Host "Building for macOS (osx-x64)..."
# Revert OutputType to Exe for cross-platform compatibility
(Get-Content -Path $projectPath) -replace '<OutputType>WinExe</OutputType>', '<OutputType>Exe</OutputType>' | Set-Content -Path $projectPath
dotnet publish $projectPath -r osx-x64 -c Release --self-contained true /p:PublishSingleFile=true

# --- Build for macOS (osx-arm64) ---
Write-Host "Building for macOS (osx-arm64)..."
dotnet publish $projectPath -r osx-arm64 -c Release --self-contained true /p:PublishSingleFile=true

Write-Host "Build process completed successfully."
Write-Host "You can find the executables in the 'Renderer/bin/Release/net6.0' directory, inside their respective runtime identifier folders." 