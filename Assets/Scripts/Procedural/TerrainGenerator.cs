using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// , typeof(ObjectContainer)
[RequireComponent(typeof(MeshFilter), typeof(ObjectContainer))]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private int _meshDensity = 64;

    [SerializeField]
    private float _scale = 1f;

    [SerializeField]
    private Material _material;

    public IMeshGenerator meshGenerator;

    public IEnumerable<IVertexModifier> VertexModifiers
    {
        get
        {
            foreach (var modifier in _modifiers)
            {
                if (modifier is IVertexModifier vertexModifier)
                {
                    yield return vertexModifier;
                }
            }
        }
    }

    [SerializeField]
    private List<ScriptableObject> _modifiers = new List<ScriptableObject>();

    [SerializeField]
    private float _radius = 10f;
    private List<Chunk> _chunks = new();

    public void Generate()
    {
        // only do this if in edit
        ValidateChunks();
        GenerateAtTransform(transform);
    }

    public void SaveMesh()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        var path = $"Assets/Meshes/{name}.asset";
        Debug.Log($"Saving mesh to {path}");
        UnityEditor.AssetDatabase.CreateAsset(mesh, path);
    }

    public void GenerateAtTransform(Transform t)
    {
        var chunk = Chunk.Create(
            transform,
            t.position,
            new QuadMeshGenerator(64, _scale),
            _material,
            $"Chunk_{_chunks.Count}"
        );

        foreach (var modifier in VertexModifiers)
        {
            chunk.ApplyModifier(modifier);
        }

        _chunks.Add(chunk);
    }

    public void GenerateRadial()
    {
        ValidateChunks();
        var chunks = GenerateRadialChunks(
            _radius,
            _scale,
            _material,
            transform.position,
            _meshDensity
        );
        Debug.Log($"Generated {chunks.Count} chunks");
        foreach (var modifier in VertexModifiers)
        {
            foreach (var chunk in chunks)
            {
                chunk.ApplyModifier(modifier);
            }
        }
        _chunks.AddRange(chunks);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var chunk in _chunks)
        {
            if (chunk != null)
            {
                Bounds bounds = chunk.GetComponent<Renderer>().bounds;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }

    private List<Chunk> GenerateRadialChunks(
        float radius,
        float chunkSize, // Desired size for each chunk
        Material material,
        Vector3 center,
        int meshDensity = 64
    )
    {
        int numChunks = Mathf.CeilToInt(2 * radius / chunkSize); // Determine number of chunks in each dimension

        List<Chunk> chunks = new();

        float meshScale = chunkSize / meshDensity; // This makes sure the mesh fits within the desired chunkSize

        // IMeshGenerator meshGenerator = new QuadMeshGenerator(meshDensity, meshScale);
        IMeshGenerator meshGenerator = new PoissonMeshGenerator(
            new Vector2(chunkSize, chunkSize),
            meshScale,
            meshScale / 2
        );
        
        for (int x = 0; x <= numChunks; x++)
        {
            for (int z = 0; z <= numChunks; z++)
            {
                var newPosition = new Vector3(x * chunkSize - radius, 0, z * chunkSize - radius);

                if (newPosition.magnitude <= radius)
                {
                    var chunk = Chunk.Create(
                        transform,
                        newPosition,
                        meshGenerator,
                        material,
                        $"Chunk_{chunks.Count}"
                    );

                    chunks.Add(chunk);
                }
            }
        }
        return chunks;
    }

    public void ClearChunks()
    {
        foreach (var chunk in _chunks)
        {
            try
            {
                if (Application.isPlaying)
                {
                    Destroy(chunk.gameObject);
                }
                else
                {
                    DestroyImmediate(chunk.gameObject);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"Failed to destroy chunk {chunk} with exception {e}");
            }
        }
        _chunks.Clear();
    }

    private void ValidateChunks()
    {
        var chunksToRemove = new List<Chunk>();
        foreach (var chunk in _chunks)
        {
            if (chunk == null)
            {
                chunksToRemove.Add(chunk);
            }
        }
        foreach (var chunk in chunksToRemove)
        {
            try
            {
                _chunks.Remove(chunk);
                if (Application.isPlaying)
                {
                    Destroy(chunk.gameObject);
                }
                else
                {
                    DestroyImmediate(chunk.gameObject);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"Failed to remove chunk {chunk} with exception {e}");
            }
        }
    }
}
