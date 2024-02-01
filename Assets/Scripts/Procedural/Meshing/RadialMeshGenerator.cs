using UnityEngine;
using System.Collections.Generic;
using g3;


// idea - instead of using a global perlin noise function with some sorts of 
// modifiers, sample the noise more like a walk or brownian motion, where
// each transformation is cumulative. So certain areas have a level of persistence
public class RadialMeshGenerator : ScriptableObject, IMeshGenerator
{
    public float radius = 10f;
    public float scale = 1f;


    public RadialMeshGenerator(float radius, float scale)
    {
        this.radius = radius;
        this.scale = scale;
        DMesh3 mesh = new DMesh3();
    }

    public void SetScale(float scale)
    {
        this.scale = scale;
    }
    
    public (Vector3[] vertices, int[] triangles, Vector2[] uvs) Generate()
    {
        int gridSize = Mathf.CeilToInt(2 * radius);
        gridSize = Mathf.Min(gridSize, 256);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate all vertices for the grid first
        for (int x = 0; x <= gridSize; x++)
        {
            for (int z = 0; z <= gridSize; z++)
            {
                // vertices.Add(new Vector3(x - _radius, 0, z - _radius));
                vertices.Add(new Vector3((x - radius) * scale, 0, (z - radius) * scale));
            }
        }

        // Create triangles only for quads inside the radius
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 quadCenter = new Vector3(x + 0.5f - radius, 0, z + 0.5f - radius);
                if (quadCenter.magnitude <= radius)
                {
                    // Compute vertex indices for this quad
                    int topLeftIndex = x * (gridSize + 1) + z;
                    int topRightIndex = (x + 1) * (gridSize + 1) + z;
                    int bottomLeftIndex = x * (gridSize + 1) + z + 1;
                    int bottomRightIndex = (x + 1) * (gridSize + 1) + z + 1;

                    // Create triangles
                    triangles.Add(topLeftIndex);
                    triangles.Add(bottomLeftIndex);
                    triangles.Add(bottomRightIndex);

                    triangles.Add(topLeftIndex);
                    triangles.Add(bottomRightIndex);
                    triangles.Add(topRightIndex);
                }
            }
        }

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < vertices.Count; i++)
        {
            uvs.Add(
                new Vector2(
                    (vertices[i].x + radius) / (2 * radius),
                    (vertices[i].z + radius) / (2 * radius)
                )
            );
        }

        return (vertices.ToArray(), triangles.ToArray(), uvs.ToArray());
    }
}
