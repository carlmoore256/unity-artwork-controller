using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneWebsocketHandler))]
public class SceneWebsocketHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneWebsocketHandler sceneWebsocketHandler = (SceneWebsocketHandler)target;
        if (GUILayout.Button("Broadcast Artworks Available"))
        {
            sceneWebsocketHandler.BroadcastArtworksAvailable();
        }
    }
}