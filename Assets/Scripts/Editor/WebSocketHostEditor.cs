using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WebSocketHost))]
public class WebSocketHostEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WebSocketHost webSocketHost = (WebSocketHost)target;
        if (GUILayout.Button("Broadcast Test"))
        {
            webSocketHost.Broadcast(new WebSocketSendMessageData("test", "test"));
        }
    }
}