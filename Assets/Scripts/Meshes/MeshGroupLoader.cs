using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

public class MeshGroupLoader : MonoBehaviour
{
    // the prefab that each mesh object will use
    [SerializeField]
    private GameObject _meshObjectPrefab;

    // material that each mesh object will use
    [SerializeField]
    private Material _meshMaterial;

    // name of the mesh
    [SerializeField]
    private string _meshName;

    [SerializeField]
    private float _scale = 0.001f;


    // the depth that is encoded into the metadata with a depth inference network
    [SerializeField]
    private float _globalDepthScale = 10.0f;

    [SerializeField]
    private bool _addColliders = true;

    // root directory under the Resources folder
    private readonly string _folderRoot = "Meshes";

    private string _meshGroupPath;
    public string MeshGroupPath
    {
        get
        {
            if (!_meshGroupPath.IsNullOrEmpty())
            {
                return _meshGroupPath;
            }
            _meshGroupPath = Path.Combine(_folderRoot, _meshName);
            return _meshGroupPath;
        }
    }

    public static MeshGroupMetadata LoadMetadataFromResources(string resourcePath)
    {
        string metadataPath = Path.Combine(resourcePath, "metadata");
        var metadataAsset = Resources.Load<TextAsset>(metadataPath);
        if (metadataAsset == null)
        {
            Debug.LogError($"Failed to load resource at path: {metadataPath}");
            return null;
        }
        string json = metadataAsset.text;
        return JsonConvert.DeserializeObject<MeshGroupMetadata>(json);
    }

    private void OnEnable()
    {
        Load();
    }

    public void Load()
    {
        transform.localScale = Vector3.one;
        Debug.Log($"MeshGroupPath: {MeshGroupPath}");
        var metadata = LoadMetadataFromResources(MeshGroupPath);
        if (metadata == null)
        {
            Debug.LogError($"Failed to load metadata for mesh group: {_meshName}");
            return;
        }

        (GameObject meshObject, Renderer renderer)[] meshObjects = new (GameObject, Renderer)[
            metadata.meshes.Length
        ];

        for (int i = 0; i < metadata.meshes.Length; i++)
        {
            var objectMetadata = metadata.meshes[i];
            meshObjects[i] = SpawnMeshObject(objectMetadata, metadata);
        }

        var bounds = new Bounds(transform.position, Vector3.zero);
        foreach ((GameObject meshObject, Renderer renderer) in meshObjects)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        var center = bounds.center;
        foreach ((GameObject meshObject, Renderer renderer) in meshObjects)
        {
            meshObject.transform.position -= center;
        }

        transform.localScale = new Vector3(_scale, _scale, _scale);
    }

    private (GameObject meshObject, Renderer renderer) SpawnMeshObject(
        MeshObjectMetadata objectMetadata,
        MeshGroupMetadata groupMetadata
    )
    {
        GameObject meshObject;
        // we can first check if the layer exists, then we might not need to load the mesh
        var meshObjectTransform = transform.Find(objectMetadata.MeshLayerName);
        if (meshObjectTransform != null)
        {
            meshObject = meshObjectTransform.gameObject;
        }
        else
        {
            var mesh = LoadMesh(objectMetadata.meshFile);
            meshObject = Instantiate(_meshObjectPrefab, transform);
            meshObject.name = objectMetadata.MeshLayerName;
            meshObject.GetOrAddComponent<MeshFilter>().mesh = mesh;
        }

        var texture = LoadTexture(objectMetadata.textureFile);
        var normalMap = LoadTexture(objectMetadata.normalFile);
        var renderer = meshObject.GetOrAddComponent<MeshRenderer>();
        renderer.material = _meshMaterial;
        renderer.material.mainTexture = texture;
        if (normalMap != null)
        {
            renderer.material.EnableKeyword("_NORMALMAP");
            renderer.material.SetTexture("_BumpMap", normalMap);
        }
        else
        {
            renderer.material.DisableKeyword("_NORMALMAP");
        }

        if (_addColliders)
        {
            if (!meshObject.TryGetComponent<MeshCollider>(out var collider))
            {
                collider = meshObject.AddComponent<MeshCollider>();
                collider.convex = true;
                collider.sharedMesh = meshObject.GetComponent<MeshFilter>().mesh;
                // meshObject.GetOrAddComponent<Rigidbody>().useGravity = false;
                // meshObject.GetOrAddComponent<MeshPhysicsController>();
            }
        }

        PositionMeshObject(meshObject, objectMetadata, groupMetadata, _globalDepthScale);
        return (meshObject, renderer);
    }

    private Texture2D LoadTexture(string filename)
    {
        string path = Path.Combine(MeshGroupPath, filename);
        path = Path.ChangeExtension(path, null);
        return Resources.Load<Texture2D>(path);
    }

    private Mesh LoadMesh(string filename)
    {
        string path = Path.Combine(MeshGroupPath, filename);
        path = Path.ChangeExtension(path, null);
        return Resources.Load<Mesh>(path);
    }

    private void PositionMeshObject(
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

    public void ClearAllChildren()
    {
        // Iterate backwards through the child list to safely remove children while iterating.
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // Check if we're in the editor or not to decide on DestroyImmediate vs. Destroy
            if (Application.isEditor)
            {
                // Use DestroyImmediate in the editor to remove the GameObject immediately.
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            else
            {
                // Use Destroy at runtime to mark the GameObject for destruction at the end of the frame.
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
