using UnityEngine;
using g3;

public interface IMeshGenerator
{
    public (Vector3[] vertices, int[] triangles, Vector2[] uvs) Generate();
}
