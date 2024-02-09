using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PhysicsInsert))]
public class PhysicsInsertEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PhysicsInsert myScript = (PhysicsInsert)target;
        if (GUILayout.Button("Insert"))
        {
            
        }
    }
}