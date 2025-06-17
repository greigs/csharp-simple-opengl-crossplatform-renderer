using Renderer.Math;

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
            Target = new Vector3(0, 0, 0);
            Up = new Vector3(0, 1, 0);
        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.LookAt(Position, Target, Up);
        }
    }
} 