using System;

namespace Renderer.Math
{
    public struct Matrix4x4
    {
        public float[,] M;

        public Matrix4x4()
        {
            M = new float[4, 4];
        }

        public static Matrix4x4 Identity()
        {
            Matrix4x4 m = new Matrix4x4();
            m.M[0, 0] = 1; m.M[1, 1] = 1; m.M[2, 2] = 1; m.M[3, 3] = 1;
            return m;
        }

        public static Matrix4x4 CreateTranslation(Vector3 v)
        {
            Matrix4x4 m = Identity();
            m.M[3, 0] = v.X;
            m.M[3, 1] = v.Y;
            m.M[3, 2] = v.Z;
            return m;
        }

        public static Matrix4x4 CreateScale(Vector3 v)
        {
            Matrix4x4 m = new Matrix4x4();
            m.M[0, 0] = v.X;
            m.M[1, 1] = v.Y;
            m.M[2, 2] = v.Z;
            m.M[3, 3] = 1;
            return m;
        }

        public static Matrix4x4 CreateRotationX(float angle)
        {
            Matrix4x4 m = Identity();
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            m.M[1, 1] = c; m.M[1, 2] = s;
            m.M[2, 1] = -s; m.M[2, 2] = c;
            return m;
        }

        public static Matrix4x4 CreateRotationY(float angle)
        {
            Matrix4x4 m = Identity();
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            m.M[0, 0] = c; m.M[0, 2] = -s;
            m.M[2, 0] = s; m.M[2, 2] = c;
            return m;
        }

        public static Matrix4x4 CreateRotationZ(float angle)
        {
            Matrix4x4 m = Identity();
            float c = (float)System.Math.Cos(angle);
            float s = (float)System.Math.Sin(angle);
            m.M[0, 0] = c; m.M[0, 1] = s;
            m.M[1, 0] = -s; m.M[1, 1] = c;
            return m;
        }

        public static Matrix4x4 CreatePerspectiveFieldOfView(float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            Matrix4x4 m = new Matrix4x4();
            float tanHalfFov = (float)System.Math.Tan(fov / 2);
            m.M[0, 0] = 1 / (aspectRatio * tanHalfFov);
            m.M[1, 1] = 1 / tanHalfFov;
            m.M[2, 2] = -(farPlane + nearPlane) / (farPlane - nearPlane);
            m.M[2, 3] = -1;
            m.M[3, 2] = -(2 * farPlane * nearPlane) / (farPlane - nearPlane);
            return m;
        }
        
        public static Matrix4x4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 zaxis = target - eye;
            zaxis.Normalize();
            Vector3 xaxis = Vector3.Cross(up, zaxis);
            xaxis.Normalize();
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);

            Matrix4x4 viewMatrix = Identity();
            viewMatrix.M[0, 0] = xaxis.X;
            viewMatrix.M[1, 0] = xaxis.Y;
            viewMatrix.M[2, 0] = xaxis.Z;
            viewMatrix.M[0, 1] = yaxis.X;
            viewMatrix.M[1, 1] = yaxis.Y;
            viewMatrix.M[2, 1] = yaxis.Z;
            viewMatrix.M[0, 2] = -zaxis.X;
            viewMatrix.M[1, 2] = -zaxis.Y;
            viewMatrix.M[2, 2] = -zaxis.Z;
            
            viewMatrix.M[3, 0] = -Vector3.Dot(xaxis, eye);
            viewMatrix.M[3, 1] = -Vector3.Dot(yaxis, eye);
            viewMatrix.M[3, 2] = Vector3.Dot(zaxis, eye);
            
            return viewMatrix;
        }

        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result.M[i, j] = a.M[i, 0] * b.M[0, j] + a.M[i, 1] * b.M[1, j] + a.M[i, 2] * b.M[2, j] + a.M[i, 3] * b.M[3, j];
                }
            }
            return result;
        }

        public static Vector3 Transform(Vector3 v, Matrix4x4 m)
        {
            float x = v.X * m.M[0, 0] + v.Y * m.M[1, 0] + v.Z * m.M[2, 0] + m.M[3, 0];
            float y = v.X * m.M[0, 1] + v.Y * m.M[1, 1] + v.Z * m.M[2, 1] + m.M[3, 1];
            float z = v.X * m.M[0, 2] + v.Y * m.M[1, 2] + v.Z * m.M[2, 2] + m.M[3, 2];
            float w = v.X * m.M[0, 3] + v.Y * m.M[1, 3] + v.Z * m.M[2, 3] + m.M[3, 3];

            if (w != 0)
            {
                x /= w;
                y /= w;
                z /= w;
            }
            return new Vector3(x, y, z);
        }
    }
} 