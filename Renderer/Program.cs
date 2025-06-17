using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Renderer;

public static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    private static void Main()
    {
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 600),
            Title = "3D Renderer"
        };

        using (var game = new Game(GameWindowSettings.Default, nativeWindowSettings))
        {
            game.Run();
        }
    }    
}