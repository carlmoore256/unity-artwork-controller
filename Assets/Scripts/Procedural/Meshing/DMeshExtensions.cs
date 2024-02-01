using Unity.VisualScripting;
using UnityEngine;
using g3;
using System.Linq;

public static class DMeshExtensions {
    public static Mesh ToUnityMesh(this DMesh3 dMesh) 
    {
        Mesh mesh = new();
        mesh.vertices = dMesh.Vertices().Select(v => v.ToVector3()).ToArray();
        mesh.triangles = dMesh.ToTriangles();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Vector3 ToVector3(this Vector3d v) 
    {
        return new Vector3((float)v.x, (float)v.y, (float)v.z);
    }

    public static int[] ToTriangles(this DMesh3 dMesh) 
    {
        int[] triangles = new int[dMesh.TriangleCount * 3];
        int i = 0;
        foreach (Index3i triangle in dMesh.Triangles())
        {
            triangles[i++] = triangle.a;
            triangles[i++] = triangle.b;
            triangles[i++] = triangle.c;
        }
        return triangles;
    }
}