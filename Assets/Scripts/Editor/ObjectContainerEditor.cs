using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectContainer))]
public class ObjectContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var container = (ObjectContainer)target;

        if (GUILayout.Button("Delete All Children"))
        {
            container.DeleteAllChildren();
        }

    }
}