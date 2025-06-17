using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Renderer.Graphics
{
    public class Model
    {
        public int Vao { get; }
        public int VertexCount { get; }

        public Model(string path)
        {
            var vertices = new List<Vector3>();
            var faces = new List<int>();

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;

                    if (parts[0] == "v")
                    {
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        vertices.Add(new Vector3(x, y, z));
                    }
                    else if (parts[0] == "f")
                    {
                        for (int i = 0; i < parts.Length - 1; i++)
                        {
                            string[] faceParts = parts[i + 1].Split('/');
                            faces.Add(int.Parse(faceParts[0]) - 1);
                        }
                    }
                }
            }

            var vertexArray = new List<Vector3>();
            foreach (var index in faces)
            {
                vertexArray.Add(vertices[index]);
            }

            VertexCount = vertexArray.Count;

            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Count * Vector3.SizeInBytes, vertexArray.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
        }
    }
} 