using OpenTK.Mathematics;

namespace Renderer.Graphics
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }

        public Camera()
        {
            Position = new Vector3(0, 0, 10);
            Target = Vector3.Zero;
            Up = Vector3.UnitY;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Target, Up);
        }
    }
} 