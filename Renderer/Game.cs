using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Renderer.Graphics;
using StbImageSharp;
using System.IO;
using OpenTK.Windowing.Common.Input;
using System.Reflection;

namespace Renderer
{
    public class Game : GameWindow
    {
        private Shader shader;
        private Model model;
        private Camera camera;

        private float yaw;
        private float pitch;
        private float zoom = 45.0f;
        private bool firstMove = true;
        private Vector2 lastMousePosition;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private static Stream GetResourceStream(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fullResourceName = $"Renderer.{resourceName.Replace('/', '.')}";
            return assembly.GetManifestResourceStream(fullResourceName) ?? throw new FileNotFoundException($"Could not find embedded resource: {fullResourceName}");
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set window icon
            try
            {
                using (var stream = GetResourceStream("Assets/icon.png"))
                {
                    var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    var windowIcon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image(image.Width, image.Height, image.Data));
                    Icon = windowIcon;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not set window icon: {e.Message}");
            }

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            using (var vertStream = GetResourceStream("Shaders/shader.vert"))
            using (var fragStream = GetResourceStream("Shaders/shader.frag"))
            {
                shader = new Shader(vertStream, fragStream);
            }

            using (var modelStream = GetResourceStream("Assets/teapot.obj"))
            {
                model = new Model(modelStream);
            }
            
            camera = new Camera();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();

            var modelMatrix = Matrix4.CreateRotationY(yaw) * Matrix4.CreateRotationX(pitch);
            var viewMatrix = Matrix4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY);
            var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(zoom), (float)Size.X / Size.Y, 0.1f, 100.0f);

            shader.SetMatrix4("model", modelMatrix);
            shader.SetMatrix4("view", viewMatrix);
            shader.SetMatrix4("projection", projectionMatrix);

            GL.BindVertexArray(model.Vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, model.VertexCount);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            if (MouseState.IsButtonDown(MouseButton.Left))
            {
                if (firstMove)
                {
                    lastMousePosition = new Vector2(MouseState.X, MouseState.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = MouseState.X - lastMousePosition.X;
                    var deltaY = MouseState.Y - lastMousePosition.Y;
                    yaw += deltaX * 0.01f;
                    pitch -= deltaY * 0.01f;
                    lastMousePosition = new Vector2(MouseState.X, MouseState.Y);
                }
            }
            else
            {
                firstMove = true;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            zoom -= e.OffsetY;
            if (zoom < 1.0f) zoom = 1.0f;
            if (zoom > 45.0f) zoom = 45.0f;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
} 