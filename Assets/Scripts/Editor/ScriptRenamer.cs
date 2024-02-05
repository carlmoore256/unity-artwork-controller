using UnityEngine;
using UnityEditor;
using System.Linq;

public class ScriptRenamer : EditorWindow
{
    [MenuItem("Tools/Script Renamer")]
    public static void ShowWindow()
    {
        GetWindow<ScriptRenamer>("Script Renamer");
    }

    private string oldScriptName = "";
    private string newScriptName = "";

    void OnGUI()
    {
        GUILayout.Label("Rename Script on GameObjects", EditorStyles.boldLabel);
        oldScriptName = EditorGUILayout.TextField("Old Script Name", oldScriptName);
        newScriptName = EditorGUILayout.TextField("New Script Name", newScriptName);

        if (GUILayout.Button("Rename"))
        {
            var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in allGameObjects)
            {
                var components = go.GetComponents<Component>().ToList();
                foreach (var component in components)
                {
                    if (component.GetType().Name == oldScriptName)
                    {
                        DestroyImmediate(component);
                        go.AddComponent(System.Type.GetType(newScriptName + ",Assembly-CSharp"));
                    }
                }
            }
        }
    }
}
