using System;
using System.Drawing;
using System.Windows.Forms;
using Renderer.Graphics;
using Renderer.Math;

namespace Renderer;

public class Form1 : Form
{
    private PictureBox pictureBox;
    private System.Windows.Forms.Timer timer;
    private Model model;
    private Camera camera;

    private float yaw = 0;
    private float pitch = 0;
    private float zoom = 5;
    private Point lastMousePosition;

    public Form1()
    {
        this.Text = "3D Renderer";
        this.Size = new Size(800, 600);

        pictureBox = new PictureBox();
        pictureBox.Dock = DockStyle.Fill;
        this.Controls.Add(pictureBox);

        model = new Model();
        model.Load("Assets/teapot.obj");

        camera = new Camera();

        timer = new System.Windows.Forms.Timer();
        timer.Interval = 16; // ~60 FPS
        timer.Tick += (s, e) => pictureBox.Invalidate();
        timer.Start();

        pictureBox.Paint += PictureBox_Paint;
        pictureBox.MouseDown += PictureBox_MouseDown;
        pictureBox.MouseMove += PictureBox_MouseMove;
        pictureBox.MouseWheel += PictureBox_MouseWheel;
    }

    private void PictureBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            lastMousePosition = e.Location;
        }
    }

    private void PictureBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            float deltaX = e.X - lastMousePosition.X;
            float deltaY = e.Y - lastMousePosition.Y;

            yaw += deltaX * 0.01f;
            pitch += deltaY * 0.01f;

            lastMousePosition = e.Location;
        }
    }

    private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
    {
        zoom -= e.Delta * 0.001f;
        if (zoom < 1) zoom = 1;
        if (zoom > 20) zoom = 20;
    }

    private void PictureBox_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.Clear(Color.Black);

        Matrix4x4 rotation = Matrix4x4.CreateRotationY(yaw) * Matrix4x4.CreateRotationX(pitch);
        camera.Position = Matrix4x4.Transform(new Vector3(0, 0, zoom), rotation);

        Matrix4x4 view = camera.GetViewMatrix();
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView((float)System.Math.PI / 4, (float)pictureBox.Width / pictureBox.Height, 0.1f, 100f);
        Matrix4x4 transform = view * projection;

        foreach (var face in model.Faces)
        {
            for (int i = 0; i < face.Length; i++)
            {
                Vector3 v1 = model.Vertices[face[i]];
                Vector3 v2 = model.Vertices[face[(i + 1) % face.Length]];

                Vector3 p1 = Matrix4x4.Transform(v1, transform);
                Vector3 p2 = Matrix4x4.Transform(v2, transform);

                float x1 = (p1.X + 1) * 0.5f * pictureBox.Width;
                float y1 = (1 - (p1.Y + 1) * 0.5f) * pictureBox.Height;
                float x2 = (p2.X + 1) * 0.5f * pictureBox.Width;
                float y2 = (1 - (p2.Y + 1) * 0.5f) * pictureBox.Height;

                e.Graphics.DrawLine(Pens.White, x1, y1, x2, y2);
            }
        }
    }
}
