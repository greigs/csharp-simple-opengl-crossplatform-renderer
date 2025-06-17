using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.WinForms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Renderer.Graphics;

namespace Renderer;

public class Form1 : Form
{
    private GLControl glControl;
    private Shader shader;
    private Model model;
    private Camera camera;

    private float yaw = 0;
    private float pitch = 0;
    private float zoom = 5;
    private Point lastMousePosition;
    private bool firstMove = true;

    public Form1()
    {
        this.Text = "3D Renderer";
        this.Size = new Size(800, 600);

        glControl = new GLControl();
        glControl.Dock = DockStyle.Fill;
        this.Controls.Add(glControl);

        glControl.Load += GlControl_Load;
        glControl.Paint += GlControl_Paint;
        glControl.Resize += GlControl_Resize;

        glControl.MouseDown += GlControl_MouseDown;
        glControl.MouseMove += GlControl_MouseMove;
        glControl.MouseWheel += GlControl_MouseWheel;
    }

    private void GlControl_Load(object sender, EventArgs e)
    {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        model = new Model("Assets/teapot.obj");
        camera = new Camera();
    }

    private void GlControl_Resize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, glControl.Width, glControl.Height);
    }

    private void GlControl_MouseDown(object sender, MouseEventArgs e)
    {
        lastMousePosition = e.Location;
        firstMove = true;
    }

    private void GlControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if (firstMove)
            {
                lastMousePosition = e.Location;
                firstMove = false;
            }
            else
            {
                var deltaX = e.X - lastMousePosition.X;
                var deltaY = e.Y - lastMousePosition.Y;
                yaw += deltaX * 0.01f;
                pitch -= deltaY * 0.01f;
                lastMousePosition = e.Location;
                glControl.Invalidate();
            }
        }
    }

    private void GlControl_MouseWheel(object sender, MouseEventArgs e)
    {
        zoom -= e.Delta * 0.01f;
        if (zoom < 1.0f) zoom = 1.0f;
        if (zoom > 45.0f) zoom = 45.0f;
        glControl.Invalidate();
    }

    private void GlControl_Paint(object sender, PaintEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        shader.Use();

        var modelMatrix = Matrix4.CreateRotationY(yaw) * Matrix4.CreateRotationX(pitch);
        var viewMatrix = Matrix4.LookAt(new Vector3(0, 0, zoom), Vector3.Zero, Vector3.UnitY);
        var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)glControl.Width / glControl.Height, 0.1f, 100.0f);

        shader.SetMatrix4("model", modelMatrix);
        shader.SetMatrix4("view", viewMatrix);
        shader.SetMatrix4("projection", projectionMatrix);

        GL.BindVertexArray(model.Vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, model.VertexCount);

        glControl.SwapBuffers();
    }
}
