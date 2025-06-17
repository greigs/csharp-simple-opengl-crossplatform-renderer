# This script builds and publishes the Renderer for Windows, macOS (Intel), and macOS (Apple Silicon).

Write-Host "Starting build process..."

# Define project path for clarity
$projectPath = "Renderer/Renderer.csproj"

# --- Build for Windows (win-x64) ---
Write-Host "Building for Windows (win-x64)..."
dotnet publish $projectPath -r win-x64 -c Release --self-contained true /p:PublishSingleFile=true
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "Renderer/bin/Release/net6.0/win-x64/publish/Assets"
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "Renderer/bin/Release/net6.0/win-x64/publish/Shaders"

# --- Function to create macOS .app bundle ---
function Create-MacAppBundle {
    param (
        [string]$runtimeId
    )
    $publishDir = "Renderer/bin/Release/net6.0/$runtimeId/publish"
    $appName = "Renderer"
    $appBundleName = "$appName.app"
    $appBundlePath = "$publishDir/$appBundleName"

    Write-Host "Creating .app bundle for $runtimeId..."

    # Clean up previous bundle
    Remove-Item -Recurse -Force -Path $appBundlePath -ErrorAction SilentlyContinue

    # Create the bundle structure
    New-Item -ItemType Directory -Force -Path "$appBundlePath/Contents/MacOS"
    New-Item -ItemType Directory -Force -Path "$appBundlePath/Contents/Resources"

    # Move the essential files into the bundle
    Move-Item -Path "$publishDir/$appName" -Destination "$appBundlePath/Contents/MacOS/"
    Move-Item -Path "$publishDir/libglfw.3.dylib" -Destination "$appBundlePath/Contents/MacOS/"
    Copy-Item -Path "./Info.plist" -Destination "$appBundlePath/Contents/"

    # Get a list of everything EXCEPT the .app bundle
    $filesToRemove = Get-ChildItem -Path $publishDir -Exclude $appBundleName

    # Remove all other files and folders from the publish directory
    foreach ($file in $filesToRemove) {
        Remove-Item -Recurse -Force $file.FullName
    }
    
    Write-Host "Successfully created and cleaned $appBundlePath"
}

# --- Build for macOS (osx-x64) ---
Write-Host "Building for macOS (osx-x64)..."
dotnet publish $projectPath -r osx-x64 -c Release --self-contained true
Create-MacAppBundle -runtimeId "osx-x64"

# --- Build for macOS (osx-arm64) ---
Write-Host "Building for macOS (osx-arm64)..."
dotnet publish $projectPath -r osx-arm64 -c Release --self-contained true
Create-MacAppBundle -runtimeId "osx-arm64"

Write-Host "Build process completed successfully."
Write-Host "You can find the build artifacts in the 'Renderer/bin/Release/net6.0' directory."
Write-Host "Windows: win-x64/publish"
Write-Host "macOS: osx-x64/publish/Renderer.app and osx-arm64/publish/Renderer.app" 