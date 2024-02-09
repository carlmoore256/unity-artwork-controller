using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

#if UNITY_EDITOR
// for use in editor-only scenes
public class MeshGroupImporter : MonoBehaviour
{
    [Header("Resource Location")]
    public static string folderRoot = "Meshes";
    public string meshGroupName;
    public string MeshGroupRoot => Path.Combine("Assets", folderRoot, meshGroupName);
    public string namePrefix = "MeshGroup__";

    public GameObject meshGroupPrefab;
    public GameObject meshObjectPrefab;
    public Material meshMaterial;

    public float scale = 0.1f;
    public float depthScale = 0.05f;
    public float globalDepthScale = 10.0f;
    public bool addColliders = false;

    private static string GetMeshGroupRoot(string meshGroupName)
    {
        return Path.Combine("Assets", folderRoot, meshGroupName);
    }

    private static Texture2D LoadTextureFromAssets(string meshGroupRoot, string filename)
    {
        string path = Path.Combine(meshGroupRoot, filename);
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    private static Mesh LoadMeshFromAssets(string meshGroupRoot, string filename)
    {
        string path = Path.Combine(meshGroupRoot, filename);
        return AssetDatabase.LoadAssetAtPath<Mesh>(path);
    }

    public static MeshGroupMetadata LoadGroupMetadata(string meshGroupName)
    {
        string meshGroupRoot = GetMeshGroupRoot(meshGroupName);

        string metadataPath = Path.Combine(meshGroupRoot, "metadata.json");

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
        string meshGroupRoot,
        MeshObjectMetadata objectMetadata,
        MeshGroupMetadata groupMetadata,
        Transform parent,
        bool addCollider = false
    )
    {
        var bbox = objectMetadata.bbox;
        var texture = LoadTextureFromAssets(meshGroupRoot, objectMetadata.textureFile);
        var normalMap = LoadTextureFromAssets(meshGroupRoot, objectMetadata.normalFile);
        var mesh = LoadMeshFromAssets(meshGroupRoot, objectMetadata.meshFile);
        var meshObject = Instantiate(meshObjectPrefab, parent);
        meshObject.name = objectMetadata.MeshLayerName;
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
            renderer.material.SetFloat("_PhaseFactor", Random.Range(0.0f, Mathf.PI * 2.0f));
        }

        if (addCollider)
        {
            Debug.Log("Adding collider");
            var meshCollider = meshObject.GetOrAddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            meshObject.GetOrAddComponent<Rigidbody>().useGravity = false;
            meshObject.GetOrAddComponent<MeshPhysicsController>();
        }

        PositionMeshObject(meshObject, objectMetadata, groupMetadata, globalDepthScale);
        // meshObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    private static void PositionMeshObject(
        GameObject meshObject,
        MeshObjectMetadata metadata,
        MeshGroupMetadata groupMetadata,
        float globalDepthScale = 10.0f
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
        // var metadata = LoadGroupMetadata(meshGroupName);
        var metadata = MeshGroupMetadataExtensions.LoadFromAssets(meshGroupName);
        Debug.Log($"Loaded metadata: {metadata.name}");

        string name = $"{namePrefix}{metadata.name}";
        RemoveExistingChildren(name);

        var meshGroupParent = Instantiate(meshGroupPrefab, transform);
        meshGroupParent.name = name;
        meshGroupParent.GetOrAddComponent<MeshSegmentsArtwork>();

        string meshGroupRoot = GetMeshGroupRoot(metadata.name);

        for (int i = 0; i < metadata.meshes.Length; i++)
        {
            var meshMetadata = metadata.meshes[i];
            SpawnMeshObject(
                meshGroupRoot,
                meshMetadata,
                metadata,
                meshGroupParent.transform,
                addColliders
            );
        }

        meshGroupParent.transform.localScale = new Vector3(scale, scale, scale * depthScale);

        // center the object
        // get bounds of all objects
        var bounds = new Bounds(meshGroupParent.transform.position, Vector3.zero);
        foreach (Renderer renderer in meshGroupParent.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Debug.Log($"Bounds: {bounds}");

        // get center of bounds
        var center = bounds.center;

        // transform each object so that the center is at the origin
        foreach (Transform child in meshGroupParent.transform)
        {
            child.transform.position -= center;
        }
        // transform.position = new Vector3(-center.x, -center.y, -center.z);
    }
}

public static class MeshGroupMetadataExtensions
{
    public static MeshGroupMetadata LoadFromAssets(
        string meshGroupName,
        string meshesRoot = "Assets/Meshes"
    )
    {
        string groupRoot = Path.Combine(meshesRoot, meshGroupName);
        string metadataPath = Path.Combine(groupRoot, "metadata.json");
        // Ensure the asset is imported and up-to-date
        AssetDatabase.ImportAsset(metadataPath, ImportAssetOptions.ForceUpdate);
        TextAsset metadataAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(metadataPath);
        if (metadataAsset == null)
        {
            Debug.LogError("Failed to load metadata.json");
            return null;
        }
        string json = metadataAsset.text;
        return JsonConvert.DeserializeObject<MeshGroupMetadata>(json);
    }
}

#endif
