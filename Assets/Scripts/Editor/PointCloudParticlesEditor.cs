using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointCloudParticles))]
public class PointCloudParticlesEditor : Editor
{

    string _artworkName = "shasta";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // make a text entry
        _artworkName = EditorGUILayout.TextField("Artwork Name", _artworkName);

        PointCloudParticles script = (PointCloudParticles)target;
        if (GUILayout.Button("Load Artwork"))
        {
            script.LoadArtwork(_artworkName);
        }
    }
}