using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGroupLoader))]
public class MeshGroupLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshGroupLoader myScript = (MeshGroupLoader)target;
        if (GUILayout.Button("Load Mesh Group"))
        {
            myScript.Load();
        }

        if (GUILayout.Button("Clear"))
        {
            myScript.ClearAllChildren();
        }
    }
}