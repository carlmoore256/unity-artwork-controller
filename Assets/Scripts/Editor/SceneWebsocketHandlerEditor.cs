using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArtworkSceneWebsocketService))]
public class SceneWebsocketHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ArtworkSceneWebsocketService sceneWebsocketHandler = (ArtworkSceneWebsocketService)target;
        if (GUILayout.Button("Broadcast Artworks Available"))
        {
            // sceneWebsocketHandler.BroadcastArtworksAvailable();
        }
    }
}