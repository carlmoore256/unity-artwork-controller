using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkInfo))]
public class NetworkInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NetworkInfo networkInfo = (NetworkInfo)target;
        if (GUILayout.Button("Get Local IP Address"))
        {
            networkInfo.ipAddress = NetworkInfo.GetLocalIPAddress();
        }
    }
}
