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

        public Model(Stream modelStream)
        {
            var vertices = new List<Vector3>();
            var faces = new List<int>();

            using (StreamReader reader = new StreamReader(modelStream))
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
                        for (int i = 1; i < parts.Length; i++)
                        {
                            string[] faceParts = parts[i].Split('/');
                            faces.Add(int.Parse(faceParts[0]) - 1);
                        }
                    }
                }
            }

            var finalVertices = new List<Vector3>();
            foreach (var face in faces)
            {
                finalVertices.Add(vertices[face]);
            }

            VertexCount = finalVertices.Count;

            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, finalVertices.Count * Vector3.SizeInBytes, finalVertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
        }
    }
} 