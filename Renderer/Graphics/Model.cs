using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Renderer.Math;

namespace Renderer.Graphics
{
    public class Model
    {
        public List<Vector3> Vertices { get; } = new List<Vector3>();
        public List<int[]> Faces { get; } = new List<int[]>();

        public void Load(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                    {
                        continue;
                    }

                    if (parts[0] == "v")
                    {
                        float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        Vertices.Add(new Vector3(x, y, z));
                    }
                    else if (parts[0] == "f")
                    {
                        int[] face = new int[parts.Length - 1];
                        for (int i = 0; i < face.Length; i++)
                        {
                            string[] faceParts = parts[i + 1].Split('/');
                            face[i] = int.Parse(faceParts[0]) - 1;
                        }
                        Faces.Add(face);
                    }
                }
            }
        }
    }
} 