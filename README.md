# 3D Teapot Renderer

This project is a simple 3D renderer built with C# and OpenTK. It displays the Stanford Teapot model, which can be rotated and zoomed using the mouse. The application is cross-platform and can be built for Windows, macOS (Intel), and macOS (Apple Silicon).
![image](https://github.com/user-attachments/assets/731fae41-1283-4ed4-b8bb-045c1232eaec)

## Prerequisites

*   [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later.
*   To package the macOS `.dmg` file, you will need access to a macOS machine.

## Build Process

The project includes a PowerShell script to automate the build process for all target platforms. This script can be run on Windows, macOS, or Linux, as long as the .NET SDK is installed.

To build the application, run the following command from the root of the project:

```powershell
./build.ps1
```

This script will compile the renderer and place the build artifacts in the `Renderer/bin/Release/net6.0/` directory, organized by platform:

*   **Windows:** `win-x64/publish/` - Contains the `Renderer.exe` and required `glfw3.dll`.
*   **macOS (Intel):** `osx-x64/publish/` - Contains the `Renderer.app` bundle.
*   **macOS (Apple Silicon):** `osx-arm64/publish/` - Contains the `Renderer.app` bundle.

## Packaging for Distribution

### Windows

To distribute the Windows version, simply zip the entire contents of the `Renderer/bin/Release/net6.0/win-x64/publish/` directory.

### macOS

The macOS versions are packaged into `.dmg` (Disk Image) files, which is the standard for distribution on that platform. A separate packaging script is provided for this purpose, as it must be run on a Mac.

**Workflow:**
1.  Run the `build.ps1` script as described above.
2.  Copy the entire project folder to a macOS machine.
3.  Open a terminal, navigate to the project's root directory, and run the packaging script:

```bash
sh package-mac.sh
```

This script will find the `Renderer.app` bundles and create distributable disk images for each architecture:

*   `Renderer/bin/Release/net6.0/osx-x64/publish/Renderer-x64.dmg`
*   `Renderer/bin/Release/net6.0/osx-arm64/publish/Renderer-arm64.dmg`

These `.dmg` files are the final packages that you can distribute to your users. 
