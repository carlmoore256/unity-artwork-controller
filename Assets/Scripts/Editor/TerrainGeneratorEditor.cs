using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var generator = (TerrainGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }

        if (GUILayout.Button("Generate Radial"))
        {
            generator.GetComponent<ObjectContainer>().DeleteAllChildren();
            generator.GenerateRadial();
        }

        if (GUILayout.Button("Save Mesh")) 
        {
            generator.SaveMesh();
        }


        if (GUILayout.Button("Clear Chunks")) {
            generator.ClearChunks();
        }
    }
}