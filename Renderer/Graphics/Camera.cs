using OpenTK.Mathematics;
using System;

namespace Renderer.Graphics
{
    public class Camera
    {
        public float Distance { get; set; } = 10f;
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float Fov { get; set; } = 45f;

        public Matrix4 GetViewMatrix()
        {
            var position = CalculatePosition();
            return Matrix4.LookAt(position, Vector3.Zero, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix(float aspectRatio)
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), aspectRatio, 0.1f, 100.0f);
        }

        public void Update(float deltaX, float deltaY, float sensitivity = 0.005f)
        {
            Yaw -= deltaX * sensitivity;
            Pitch += deltaY * sensitivity;

            // Clamp pitch to avoid flipping
            if (Pitch > MathHelper.DegreesToRadians(89.0f))
                Pitch = MathHelper.DegreesToRadians(89.0f);
            if (Pitch < MathHelper.DegreesToRadians(-89.0f))
                Pitch = MathHelper.DegreesToRadians(-89.0f);
        }

        private Vector3 CalculatePosition()
        {
            var pos = Vector3.Zero;
            pos.X = Distance * (float)Math.Sin(Yaw) * (float)Math.Cos(Pitch);
            pos.Y = Distance * (float)Math.Sin(Pitch);
            pos.Z = Distance * (float)Math.Cos(Yaw) * (float)Math.Cos(Pitch);
            return pos;
        }
    }
} 