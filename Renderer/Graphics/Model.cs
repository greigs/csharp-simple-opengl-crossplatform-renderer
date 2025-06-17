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
        public int ElementCount { get; }
        public Vector3 Center { get; }

        public Model(Stream modelStream)
        {
            var tempVertices = new List<Vector3>();
            var tempFaces = new List<int[]>();

            using (StreamReader reader = new StreamReader(modelStream))
            {
                string line;
                while ((line = reader.ReadLine()!) != null)
                {
                    string[] parts = line.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;

                    if (parts[0] == "v")
                    {
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        tempVertices.Add(new Vector3(x, y, z));
                    }
                    else if (parts[0] == "f")
                    {
                        int[] face = new int[parts.Length - 1];
                        for (int i = 1; i < parts.Length; i++)
                        {
                            face[i - 1] = int.Parse(parts[i].Split('/')[0]) - 1;
                        }
                        tempFaces.Add(face);
                    }
                }
            }

            var vertexNormals = new Dictionary<int, List<Vector3>>();
            var triangleIndices = new List<uint>();

            foreach (var face in tempFaces)
            {
                for (int i = 0; i < face.Length - 2; i++)
                {
                    var i1 = (uint)face[0];
                    var i2 = (uint)face[i + 1];
                    var i3 = (uint)face[i + 2];

                    triangleIndices.AddRange(new[] { i1, i2, i3 });

                    var v1 = tempVertices[(int)i1];
                    var v2 = tempVertices[(int)i2];
                    var v3 = tempVertices[(int)i3];
                    var normal = Vector3.Cross(v2 - v1, v3 - v1).Normalized();

                    if (!vertexNormals.ContainsKey((int)i1)) vertexNormals[(int)i1] = new List<Vector3>();
                    if (!vertexNormals.ContainsKey((int)i2)) vertexNormals[(int)i2] = new List<Vector3>();
                    if (!vertexNormals.ContainsKey((int)i3)) vertexNormals[(int)i3] = new List<Vector3>();

                    vertexNormals[(int)i1].Add(normal);
                    vertexNormals[(int)i2].Add(normal);
                    vertexNormals[(int)i3].Add(normal);
                }
            }

            var vertexData = new List<float>();
            for (int i = 0; i < tempVertices.Count; i++)
            {
                var avgNormal = Vector3.Zero;
                if (vertexNormals.TryGetValue(i, out var normals))
                {
                    foreach (var n in normals) avgNormal += n;
                    avgNormal = (avgNormal / normals.Count).Normalized();
                }
                
                vertexData.AddRange(new[] { tempVertices[i].X, tempVertices[i].Y, tempVertices[i].Z, avgNormal.X, avgNormal.Y, avgNormal.Z });
            }

            ElementCount = triangleIndices.Count;

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            foreach (var vertex in tempVertices)
            {
                min.X = Math.Min(min.X, vertex.X);
                min.Y = Math.Min(min.Y, vertex.Y);
                min.Z = Math.Min(min.Z, vertex.Z);
                max.X = Math.Max(max.X, vertex.X);
                max.Y = Math.Max(max.Y, vertex.Y);
                max.Z = Math.Max(max.Z, vertex.Z);
            }
            Center = (min + max) / 2.0f;

            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Count * sizeof(float), vertexData.ToArray(), BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, triangleIndices.Count * sizeof(uint), triangleIndices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }
    }
} 