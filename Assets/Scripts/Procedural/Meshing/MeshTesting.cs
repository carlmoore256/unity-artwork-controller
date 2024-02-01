using UnityEngine;
using g3;

public class MeshTesting : MonoBehaviour
{

    public Material material;
    public DMesh3 mesh;

    private void Start()
    {
        mesh = new DMesh3();
        // MeshGenerator.GenerateCube(mesh, 1, 1, 1);
        // Mesh unityMesh = mesh.ToUnityMesh();
        // GetComponent<MeshFilter>().mesh = unityMesh;
        // GetComponent<MeshRenderer>().material = material;
    }


}