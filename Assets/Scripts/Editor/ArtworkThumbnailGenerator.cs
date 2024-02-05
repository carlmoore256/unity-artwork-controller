using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ArtworkThumbnailGenerator : AssetPostprocessor, IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get { return 0; }
    }

    // run this on build
    public void OnPreprocessBuild(BuildReport report)
    {
        GenerateAllThumbnails();
    }

    private const string ArtworksPath = "Assets/Resources/Artworks/";

    [MenuItem("Tools/Generate Artwork Thumbnails")]
    private static void GenerateAllThumbnails()
    {
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        var artworkAssets = allAssets.Where(
            asset => asset.StartsWith(ArtworksPath) && asset.EndsWith(".prefab")
        );

        foreach (string asset in artworkAssets)
        {
            GenerateThumbnailForAsset(asset);
        }

        Debug.Log("Thumbnail generation complete.");
    }

    // private static void OnPostprocessAllAssets(
    //     string[] importedAssets,
    //     string[] deletedAssets,
    //     string[] movedAssets,
    //     string[] movedFromAssetPaths
    // )
    // {
    //     foreach (string asset in importedAssets)
    //     {
    //         Debug.Log("Reimported Asset: " + asset);
    //         if (asset.StartsWith("Assets/Resources/Artworks/") && asset.EndsWith(".prefab"))
    //         {
    //             GenerateThumbnailForAsset(asset);
    //         }
    //     }
    // }

    private static void GenerateThumbnailForAsset(string assetPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab == null)
            return;
        var texture = AssetPreview.GetAssetPreview(prefab);
        // var texture = AssetPreview.GetMiniThumbnail(prefab);
        if (texture == null)
        {
            Debug.Log("Texture: " + texture + " for " + assetPath + " is null");
            return;
        }
        // convert texture to base64
        byte[] image = texture.EncodeToJPG();
        // add /thumbnails to the path
        string thumbnailPath = assetPath.Replace("Artworks", "Artworks/thumbnails");
        thumbnailPath = thumbnailPath.Replace(".prefab", ".jpg");
        // if the directory doesn't exist, create it
        string directory = Path.GetDirectoryName(thumbnailPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Debug.Log("Thumbnail generated for: " + assetPath + " at " + thumbnailPath);
        File.WriteAllBytes(thumbnailPath, image);

        // make sure the texture asset is writable

        // Example: File.WriteAllBytes(thumbnailPath, thumbnailBytes);
    }
}
