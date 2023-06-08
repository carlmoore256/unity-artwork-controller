using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SVGImporter))]
public class SVGImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SVGImporter svgImporter = (SVGImporter)target;
        if (GUILayout.Button("Load and Apply SVGs"))
        {
            svgImporter.LoadAndApplySVGs();
        }


        if (GUILayout.Button("Create Texture2d"))
        {
            Texture2D test = svgImporter.TextureFromSprite();
            AssetDatabase.CreateAsset(test, "Assets/test.asset");
        }
    }
}
