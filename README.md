[![.NET](https://github.com/greigs/csharp-opengl-crossplatform-renderer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/greigs/csharp-opengl-crossplatform-renderer/actions/workflows/dotnet.yml)

# 3D Teapot Renderer

This project is a simple 3D renderer built with C# and OpenTK. It displays the Stanford Teapot model with an animated, lit, tie-dye pattern which can be rotated and zoomed using the mouse. The application is cross-platform and can be built for Windows, macOS (Intel), and macOS (Apple Silicon). The resulting packaged applications include .net 8.0 already embedded, so it should run even if .net is not installed.

![image](https://github.com/user-attachments/assets/57a9a757-c025-44a6-93a2-44b683e1ef83)


## How it Works

The renderer uses OpenTK, a C# wrapper for the OpenGL graphics API, to achieve hardware-accelerated rendering. The core of the process involves a programmable pipeline using shaders.

### 1. Asset Loading & Normal Calculation
- The `teapot.obj` model and the GLSL shader files are embedded directly into the application executable.
- **Vertex Welding:** On startup, the `Model.cs` class parses the `.obj` file. The teapot model is defined in patches, resulting in duplicated vertices at the seams where patches meet. To create a smooth surface, these duplicate vertices are "welded" together by identifying points with identical positions and treating them as a single vertex.
- **Normal Calculation:** Since the model file does not contain vertex normals (which are essential for lighting), they are calculated manually after welding. For each unique vertex, its normal is computed by averaging the surface normals of all the triangles that share that vertex. This process results in a smooth appearance and eliminates lighting seams.
- The final unique vertex data, interleaved with positions and their corresponding calculated normals, is uploaded to the GPU into a Vertex Buffer Object (VBO). An Element Buffer Object (EBO) is also created to allow for efficient indexed drawing.

### 2. The Rendering Loop
The application runs a continuous loop that performs the following steps for each frame:

#### a. Transformation (The MVP Matrix)
The position of each vertex is transformed from its local model space into its final screen position using a series of matrix multiplications (Model-View-Projection).
- **Model Matrix:** Translates the teapot to the world origin and applies rotation.
- **View Matrix:** Controlled by an orbital camera (`Camera.cs`). Mouse movements rotate the camera around the teapot, and the scroll wheel changes its distance.
- **Projection Matrix:** Applies perspective to create the illusion of depth.

#### b. Shader Execution
The MVP matrix, vertex data, and other uniforms are passed to the shaders running on the GPU.
- **`shader.vert` (Vertex Shader):** This program runs for every vertex. It transforms the vertex position using the MVP matrix and also transforms the vertex normal into world space for lighting calculations.
- **`shader.frag` (Fragment Shader):** This program runs for every pixel the teapot covers. It receives the world-space position and normal from the vertex shader.
    - **Lighting:** It calculates Blinn-Phong lighting (ambient, diffuse, and specular components) based on the surface normal and the positions of the camera and a fixed light source. This gives the teapot its 3D appearance.
    - **Color:** It calculates a procedural, animated tie-dye pattern using trigonometric functions and a `time` uniform that is updated every frame.
    - The final pixel color is the result of multiplying the lighting value by the tie-dye color.

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
