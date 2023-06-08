using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class SVGLayer
{
    public string file;
    public float[] offset;

    public Vector2 OffsetVector
    {
        get
        {
            return new Vector3(offset[0], offset[1]);
        }
    }
}

#if UNITY_EDITOR
public class ApplySVGOffsets : MonoBehaviour
{

    [MenuItem("Tools/Apply SVG Offsets")]
    public static void ApplyOffsets()
    {
        // Load the JSON file
        string jsonFilePath = AssetDatabase.GetAssetPath(Resources.Load("your_offsets_file_name_without_extension") as TextAsset);
        string jsonString = File.ReadAllText(jsonFilePath);

        // Deserialize the JSON data
        List<SVGLayer> svgOffsets = JsonConvert.DeserializeObject<List<SVGLayer>>(jsonString);

        // Iterate over each SVGOffset object and apply the transformation
        foreach (SVGLayer svgOffset in svgOffsets)
        {
            // Find the GameObject based on the file name
            GameObject go = GameObject.Find(svgOffset.file);
            if (go != null)
            {
                // Apply the offset
                go.transform.position = svgOffset.OffsetVector;
            }
            else
            {
                Debug.LogWarning($"GameObject not found: {svgOffset.file}");
            }
        }
    }
}

#endif