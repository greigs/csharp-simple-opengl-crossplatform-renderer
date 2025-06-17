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
        private Shader? shader;
        private Model? model;
        private Camera? camera;
        private Vector3 lightPos = new Vector3(5.0f, 5.0f, 5.0f);

        private float rotationAngle = 0.0f;
        private float time = 0.0f;

        private bool firstMove = true;
        private Vector2 lastMousePosition;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            UpdateFrequency = 60.0;
            Title = "Renderer";
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

            var modelMatrix = Matrix4.CreateRotationY(rotationAngle) * Matrix4.CreateTranslation(-model!.Center);
            if (shader == null) throw new Exception("Shader is null");
            if (model == null) throw new Exception("Model is null");
            if (camera == null) throw new Exception("Camera is null");

            shader!.Use();
            shader!.SetMatrix4("model", modelMatrix);
            shader!.SetMatrix4("view", camera.GetViewMatrix());
            shader!.SetMatrix4("projection", camera.GetProjectionMatrix((float)Size.X / Size.Y));
            shader!.SetFloat("time", time);
            shader!.SetVector3("viewPos", camera.Position);
            shader!.SetVector3("lightPos", lightPos);

            GL.BindVertexArray(model!.Vao);
            GL.DrawElements(PrimitiveType.Triangles, model.ElementCount, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            rotationAngle += MathHelper.DegreesToRadians(15.0f) * (float)e.Time;
            time += (float)e.Time;
            
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
                    camera!.Update(deltaX, deltaY);
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
            camera!.Distance -= e.OffsetY;
            if (camera!.Distance < 2.0f) camera!.Distance = 2.0f;
            if (camera!.Distance > 50.0f) camera!.Distance = 50.0f;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
    }
} 