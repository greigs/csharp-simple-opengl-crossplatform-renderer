# 3D Teapot Renderer

This project is a simple 3D renderer built with C# and OpenTK. It displays the Stanford Teapot model, which can be rotated and zoomed using the mouse. The application is cross-platform and can be built for Windows, macOS (Intel), and macOS (Apple Silicon). The resulting packaged applications include .net 8.0 already embedded, so it should run even if .net is not installed.

![image](https://github.com/user-attachments/assets/731fae41-1283-4ed4-b8bb-045c1232eaec)

## How it Works

The renderer uses OpenTK, a C# wrapper for the OpenGL graphics API, to achieve hardware-accelerated rendering. The core of the process involves a programmable pipeline using shaders.

### 1. Asset Loading
- The `teapot.obj` model and the GLSL shader files are embedded directly into the application executable.
- On startup, the `Model.cs` class parses the `.obj` file's vertices and calculates the model's geometric center. This data is then uploaded to the GPU into a Vertex Buffer Object (VBO).

### 2. The Rendering Loop
The application runs a continuous loop that performs the following steps for each frame:

#### a. Transformation (The MVP Matrix)
The position of each vertex is transformed from its local model space into its final screen position using a series of matrix multiplications. This is known as the Model-View-Projection (MVP) matrix.
- **Model Matrix:** This matrix first translates the teapot so its geometric center is at the world origin `(0,0,0)`, ensuring it rotates around its center.
- **View Matrix:** This is controlled by the `Camera.cs` class, which implements an orbital camera. It positions the "viewer" at a certain distance and orientation from the world origin, making it look at the teapot. Mouse movements rotate the camera around the teapot, and the scroll wheel changes its distance.
- **Projection Matrix:** This matrix applies perspective, making parts of the model that are further away appear smaller, which creates the illusion of depth.

#### b. Shader Execution
The MVP matrix and vertex data are passed to the shaders running on the GPU.
- **`shader.vert` (Vertex Shader):** This program runs for every vertex in the model. Its primary job is to apply the MVP matrix transformation to each vertex's position.
- **`shader.frag` (Fragment Shader):** After the vertices are transformed, this program runs for every pixel that the teapot covers on the screen. It determines the final color of the pixel. In this project, it's very simple: it just outputs a constant white color, giving the teapot its solid appearance.

This entire process repeats every frame, allowing for smooth animation and interaction.

## Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.
*   To package the macOS `.dmg` file, you will need access to a macOS machine.

## Build Process

The project includes a PowerShell script to automate the build process for all target platforms. This script can be run on Windows, macOS, or Linux, as long as the .NET SDK is installed.

To build the application, run the following command from the root of the project:

```powershell
./build.ps1
```

This script will compile the renderer and place the build artifacts in the `Renderer/bin/Release/net8.0/` directory, organized by platform:

*   **Windows:** `win-x64/publish/` - Contains the `Renderer.exe` and required `glfw3.dll`.
*   **macOS (Intel):** `osx-x64/publish/` - Contains the `Renderer.app` bundle.
*   **macOS (Apple Silicon):** `osx-arm64/publish/` - Contains the `Renderer.app` bundle.

## Packaging for Distribution

### Windows

To distribute the Windows version, simply zip the entire contents of the `Renderer/bin/Release/net8.0/win-x64/publish/` directory.

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

*   `Renderer/bin/Release/net8.0/osx-x64/publish/Renderer-x64.dmg`
*   `Renderer/bin/Release/net8.0/osx-arm64/publish/Renderer-arm64.dmg`

These `.dmg` files are the final packages that you can distribute to your users. 
