using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MeshObjectMetadata
{
    public string meshFile;
    public string textureFile;
    public string normalFile;
    public float[] bbox;
    public float area;
    public float sampleDensity;
    public float simplifyEps;
    public float minSampleDist;
    public int numVerts;
    public float globalDepth;
}

public class MeshGroupMetadata
{
    public string name;
    public string imageFile;
    public MeshObjectMetadata[] meshes;
    public float[] bbox;
    public int numPaths;
    public int numMeshes;
    public int numVerts;
    public float area;
    public int width;
    public int height;
}

#if UNITY_EDITOR
public class MeshGroupImporter : MonoBehaviour
{
    [Header("Resource Location")]
    public string folderRoot = "Meshes";
    public string meshGroupName;
    public string MeshGroupRoot => Path.Combine("Assets", folderRoot, meshGroupName);

    // [Header("Load Parameters")]
    // public float offsetScalar = 0.01f;

    public GameObject meshGroupPrefab;
    public GameObject meshObjectPrefab;
    public Material meshMaterial;

    public float scale = 0.1f;
    public float depthScale = 0.05f;
    public float globalDepthScale = 10.0f;
    public bool addColliders = false;

    private Texture2D LoadTexture(string filename)
    {
        string path = Path.Combine(MeshGroupRoot, filename);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private Mesh LoadMesh(string filename)
    {
        string path = Path.Combine(MeshGroupRoot, filename);
        return AssetDatabase.LoadAssetAtPath<Mesh>(path);
    }

    public MeshGroupMetadata LoadGroupMetadata()
    {
        string metadataPath = Path.Combine(MeshGroupRoot, "metadata.json");

        // Ensure the asset is imported and up-to-date
        AssetDatabase.ImportAsset(metadataPath, ImportAssetOptions.ForceUpdate);

        // Read the text from the asset
        TextAsset metadataAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(metadataPath);
        if (metadataAsset == null)
        {
            Debug.LogError("Failed to load metadata.json");
            return null;
        }

        string json = metadataAsset.text;
        return JsonConvert.DeserializeObject<MeshGroupMetadata>(json);
    }

    private void RemoveExistingChildren(string name)
    {
        var existing = transform.Find(name);
        if (existing != null)
        {
            DestroyImmediate(existing.gameObject);
        }
    }

    public void ClearAllChildren()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void SpawnMeshObject(
        MeshObjectMetadata metadata,
        Transform parent,
        MeshGroupMetadata groupMetadata,
        bool addCollider = false
    )
    {
        var bbox = metadata.bbox;
        var texture = LoadTexture(metadata.textureFile);
        var normalMap = LoadTexture(metadata.normalFile);
        var mesh = LoadMesh(metadata.meshFile);
        var meshObject = Instantiate(meshObjectPrefab, parent);
        // meshObject.name = $"MeshObject__{metadata.meshFile}";
        meshObject.GetOrAddComponent<MeshFilter>().mesh = mesh;

        var renderer = meshObject.GetOrAddComponent<MeshRenderer>();
        renderer.material = meshMaterial;
        renderer.material.mainTexture = texture;

        // Set the normal map
        if (normalMap != null)
        {
            renderer.material.SetTexture("_BumpMap", normalMap); // Set normal map
            renderer.material.EnableKeyword("_NORMALMAP"); // Enable normal map keyword
        }

        if (addCollider)
        {
            Debug.Log("Adding collider");
            var meshCollider = meshObject.GetOrAddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            meshObject.GetOrAddComponent<Rigidbody>().useGravity = false;
        }

        PositionMeshObject(meshObject, metadata, groupMetadata);
        // meshObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void PositionMeshObject(
        GameObject meshObject,
        MeshObjectMetadata metadata,
        MeshGroupMetadata groupMetadata
    )
    {
        // apply translations based on bbox
        var bbox = metadata.bbox;
        float minX = bbox[0];
        float maxX = bbox[1];
        float minY = bbox[2];
        float maxY = bbox[3];
        var height = maxY - minY;
        meshObject.transform.position = new Vector3(
            -minX,
            groupMetadata.bbox[3] - minY - height,
            metadata.globalDepth * globalDepthScale
        );
    }

    public void ImportMeshGroup()
    {
        var metadata = LoadGroupMetadata();
        Debug.Log($"Loaded metadata: {metadata.name}");

        string name = $"MeshGroup__{metadata.name}";
        RemoveExistingChildren(name);

        var meshGroupParent = Instantiate(meshGroupPrefab, transform);
        meshGroupParent.name = name;

        foreach (var meshMetadata in metadata.meshes)
        {
            SpawnMeshObject(meshMetadata, meshGroupParent.transform, metadata, addColliders);
        }

        meshGroupParent.transform.localScale = new Vector3(scale, scale, scale * depthScale);
    }
}
#endif
