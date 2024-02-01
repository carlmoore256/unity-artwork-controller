using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public Mesh mesh;

    private Mesh _baseMesh;

    private IMeshGenerator[] _lodGenerators = new IMeshGenerator[4];

    private List<IVertexModifier> _vertexModifiers = new List<IVertexModifier>();

    private void OnEnable()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log($"Got component {mesh}");
    }

    public void AddModifier(IVertexModifier modifier)
    {
        _vertexModifiers.Add(modifier);
    }

    public void ApplyModifier(IVertexModifier modifier)
    {
        mesh.vertices = modifier.Modify(mesh.vertices, transform.position);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void Generate(IMeshGenerator generator)
    {
        if (!mesh)
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }
        mesh.Clear();
        (Vector3[] vertices, int[] triangles, Vector2[] uvs) = generator.Generate();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.SetVertices(new List<Vector3>(vertices));

        mesh.uv = uvs;
        mesh.RecalculateNormals();
        // mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public static Chunk Create(
        Transform parent,
        Vector3 position,
        IMeshGenerator meshGenerator,
        Material material,
        string id
    )
    {
        var go = new GameObject(id);

        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshCollider>();

        Chunk chunk = go.AddComponent<Chunk>();
        chunk.Generate(meshGenerator);

        go.transform.parent = parent;
        go.transform.position = position;

        go.GetComponent<MeshRenderer>().material = material;

        return chunk;
    }
}
