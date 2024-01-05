using System.Collections.Generic;
using Pcx;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PointCloudMesh : MonoBehaviour
{
    private Mesh _mesh;
    public Material pointCloudMaterial;
    public float pointDensity = 0.1f;

    public void LoadArtwork(string artworkName)
    {
        PointCloudArtwork artwork = new PointCloudArtwork(artworkName, pointDensity);
        List<Color> colors = artwork.GetColors();
        List<Vector3> positions = artwork.GetPositions(new Vector3(0.001f, 0.001f, 0.001f));
        UpdateMesh(positions, colors);
#if UNITY_EDITOR
        SaveMesh(artworkName);
#endif

        // create the point cloud data (I don't think we'll need this)
        // PointCloudData pointCloudData = new PointCloudData();
        // pointCloudData.Initialize(positions, colors.ConvertAll(c => (Color32)c));
        // GetComponent<MyPointCloudRenderer>().sourceData = pointCloudData;
    }

    private void SaveMesh(string meshName)
    {
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(_mesh, "Assets/PointClouds/" + meshName + ".asset");
#endif
    }

    void UpdateMesh(List<Vector3> vertices, List<Color> colors)
    {
        _mesh = new Mesh();
        _mesh.Clear();
        _mesh.vertices = vertices.ToArray();
        _mesh.colors = colors.ToArray();
        // Since we're just using vertices as points, we don't need to define triangles
        int[] indices = new int[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            indices[i] = i;
        }
        _mesh.SetIndices(indices, MeshTopology.Points, 0);
        _mesh.RecalculateBounds();
        GetComponent<MeshRenderer>().material = pointCloudMaterial;
        GetComponent<MeshFilter>().mesh = _mesh;
    }
}
