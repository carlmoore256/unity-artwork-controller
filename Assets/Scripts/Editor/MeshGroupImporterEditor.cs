using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGroupImporter))]
public class MeshGroupImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshGroupImporter meshGroupImporter = (MeshGroupImporter)target;

        if (GUILayout.Button("Import"))
        {
            meshGroupImporter.ImportMeshGroup();
            
        }

        if (GUILayout.Button("Clear"))
        {
            meshGroupImporter.ClearAllChildren();
        }
    }
}