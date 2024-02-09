using System.IO;
using Newtonsoft.Json;
using UnityEngine;

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

    [JsonIgnore]
    public string MeshLayerName => meshFile.Split("/")[0]; 
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
