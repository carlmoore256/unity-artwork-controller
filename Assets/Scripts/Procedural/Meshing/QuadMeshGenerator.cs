using UnityEngine;
using System.Collections.Generic;

public class QuadMeshGenerator : IMeshGenerator
{
    private int _gridSize = 10;
    private float _scale = 1f;

    public QuadMeshGenerator(int gridSize, float scale)
    {
        this._gridSize = gridSize;
        this._scale = scale;
    }

    public (Vector3[] vertices, int[] triangles, Vector2[] uvs) Generate()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate all vertices for the grid first
        for (int x = 0; x <= _gridSize; x++)
        {
            for (int z = 0; z <= _gridSize; z++)
            {
                float offset = _gridSize * _scale * 0.5f;
                vertices.Add(new Vector3((x * _scale) - offset, 0, (z * _scale) - offset));
                // vertices.Add(new Vector3(x * scale - offset, 0, z * scale - offset) + position);
            }
        }

        // Create triangles for every quad
        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                // Compute vertex indices for this quad
                int topLeftIndex = x * (_gridSize + 1) + z;
                int topRightIndex = (x + 1) * (_gridSize + 1) + z;
                int bottomLeftIndex = x * (_gridSize + 1) + z + 1;
                int bottomRightIndex = (x + 1) * (_gridSize + 1) + z + 1;

                // Create triangles
                triangles.Add(topLeftIndex);
                triangles.Add(bottomLeftIndex);
                triangles.Add(bottomRightIndex);

                triangles.Add(topLeftIndex);
                triangles.Add(bottomRightIndex);
                triangles.Add(topRightIndex);
            }
        }

        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < vertices.Count; i++)
        {
            uvs.Add(
                new Vector2(vertices[i].x / (_gridSize * _scale), vertices[i].z / (_gridSize * _scale))
            );
        }

        return (vertices.ToArray(), triangles.ToArray(), uvs.ToArray());
    }
}
