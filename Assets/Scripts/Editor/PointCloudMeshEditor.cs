using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PointCloudMesh))]
public class PointCloudMeshEditor : Editor
{
    string _artworkName = "shasta";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _artworkName = EditorGUILayout.TextField("Artwork Name", _artworkName);

        PointCloudMesh script = (PointCloudMesh)target;
        if (GUILayout.Button("Load Artwork"))
        {
            script.LoadArtwork(_artworkName);
        }
    }
}
