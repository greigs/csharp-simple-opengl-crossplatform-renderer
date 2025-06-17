using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;

namespace Renderer.Graphics
{
    public class Vector3EqualityComparer : IEqualityComparer<Vector3>
    {
        private readonly float _tolerance;

        public Vector3EqualityComparer(float tolerance)
        {
            _tolerance = tolerance;
        }

        public bool Equals(Vector3 v1, Vector3 v2)
        {
            return Math.Abs(v1.X - v2.X) < _tolerance &&
                   Math.Abs(v1.Y - v2.Y) < _tolerance &&
                   Math.Abs(v1.Z - v2.Z) < _tolerance;
        }

        public int GetHashCode(Vector3 v)
        {
            return HashCode.Combine(
                (int)(v.X / _tolerance),
                (int)(v.Y / _tolerance),
                (int)(v.Z / _tolerance)
            );
        }
    }

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

            // Weld vertices by position
            var uniqueVertices = new List<Vector3>();
            var remap = new int[tempVertices.Count];
            var positionToUniqueIndex = new Dictionary<Vector3, int>(new Vector3EqualityComparer(1e-4f));

            for(int i = 0; i < tempVertices.Count; i++)
            {
                if(positionToUniqueIndex.TryGetValue(tempVertices[i], out int uniqueIndex))
                {
                    remap[i] = uniqueIndex;
                }
                else
                {
                    uniqueIndex = uniqueVertices.Count;
                    positionToUniqueIndex.Add(tempVertices[i], uniqueIndex);
                    uniqueVertices.Add(tempVertices[i]);
                    remap[i] = uniqueIndex;
                }
            }

            var triangleIndices = new List<uint>();
            foreach (var face in tempFaces)
            {
                if (face.Length == 3)
                {
                    uint i1 = (uint)remap[face[0]];
                    uint i2 = (uint)remap[face[1]];
                    uint i3 = (uint)remap[face[2]];
                    triangleIndices.AddRange(new[] { i1, i2, i3 });
                }
                else if (face.Length == 4)
                {
                    uint i1 = (uint)remap[face[0]];
                    uint i2 = (uint)remap[face[1]];
                    uint i3 = (uint)remap[face[2]];
                    uint i4 = (uint)remap[face[3]];
                    triangleIndices.AddRange(new[] { i1, i2, i3 });
                    triangleIndices.AddRange(new[] { i1, i3, i4 });
                }
            }
            
            var vertexNormals = new Dictionary<int, List<Vector3>>();
            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                var i1 = (int)triangleIndices[i];
                var i2 = (int)triangleIndices[i + 1];
                var i3 = (int)triangleIndices[i + 2];

                var v1 = uniqueVertices[i1];
                var v2 = uniqueVertices[i2];
                var v3 = uniqueVertices[i3];

                var normal = Vector3.Cross(v2 - v1, v3 - v1).Normalized();

                if (!vertexNormals.ContainsKey(i1)) vertexNormals[i1] = new List<Vector3>();
                if (!vertexNormals.ContainsKey(i2)) vertexNormals[i2] = new List<Vector3>();
                if (!vertexNormals.ContainsKey(i3)) vertexNormals[i3] = new List<Vector3>();

                vertexNormals[i1].Add(normal);
                vertexNormals[i2].Add(normal);
                vertexNormals[i3].Add(normal);
            }

            var vertexData = new List<float>();
            for (int i = 0; i < uniqueVertices.Count; i++)
            {
                var avgNormal = Vector3.Zero;
                if (vertexNormals.TryGetValue(i, out var normals))
                {
                    foreach (var n in normals) avgNormal += n;
                    avgNormal = (avgNormal / normals.Count).Normalized();
                }

                vertexData.AddRange(new[] { uniqueVertices[i].X, uniqueVertices[i].Y, uniqueVertices[i].Z, avgNormal.X, avgNormal.Y, avgNormal.Z });
            }

            ElementCount = triangleIndices.Count;

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            foreach (var vertex in uniqueVertices)
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