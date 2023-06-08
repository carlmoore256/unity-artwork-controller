using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using UnityEngine.Rendering;



#if UNITY_EDITOR
public class SVGImporter : MonoBehaviour
{
    public string folderPath;
    private string layersJSON = "layers.json";

    public Transform layerParent;

    public float offsetScalar = 0.01f;

    public Sprite testSprite;


    public Vector3 bgOffset = new Vector3(0.05f, 0.1f, 0);
    public Vector2 bgScale = new Vector2(1, 1);

    public void LoadAndApplySVGs()
    {
        if (layerParent == null) {
            // spawn an empty gameobject
            layerParent = new GameObject("Layers").transform;
            layerParent.SetParent(transform);
            layerParent.transform.localPosition = Vector3.zero;
        }

        layerParent.name = "Artwork__" + folderPath.Split('/').Last();
        
        // if (layerParent.GetComponent<MotifMotionController>() == null) {
        //     layerParent.gameObject.AddComponent<MotifMotionController>();
        // }

        if (layerParent.GetComponent<Artwork>() == null) {
            layerParent.gameObject.AddComponent<Artwork>();
        }

        // Load the JSON file
        string layersPath = Path.Combine("Assets", folderPath, "Layers");
        string jsonFilePath = Path.Combine(layersPath, layersJSON);
        string jsonString = File.ReadAllText(jsonFilePath);
        
        
        // Deserialize the JSON data
        List<SVGLayer> svgLayers = JsonConvert.DeserializeObject<List<SVGLayer>>(jsonString);

        if (svgLayers.Count == 0)
        {
            Debug.LogError("No SVG offsets found!");
            return;
        }


        // remove any child objects of layerParent
        List<Transform> childrenToDestroy = new List<Transform>();
        foreach (Transform child in layerParent)
        {
            childrenToDestroy.Add(child);
        }

        foreach (Transform child in childrenToDestroy)
        {
            DestroyImmediate(child.gameObject);
        }

        
        // find all layers with "image" in name
        List<SVGLayer> imageLayers = svgLayers.Where(layer => layer.file.StartsWith("image")).ToList();

        List<GameObject> imageObjects = new List<GameObject>();

        foreach(SVGLayer layer in imageLayers)
        {
            UnityEngine.Debug.Log(layer.file);
            GameObject layerObject = SpawnSVGLayer(layersPath, layer);
            layerObject.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            imageObjects.Add(layerObject);

            // layerObject.transform.localScale = layerObject.transform.localScale + new Vector3(0.001f, 0.001f, 0);
            Vector3 newScale = layerObject.transform.localScale;
            newScale.x *= bgScale.x;
            newScale.y *= bgScale.y;
            layerObject.transform.localScale = newScale;
            layerObject.transform.localPosition = layerObject.transform.localPosition + bgOffset;


            if (!layer.file.Contains("orig"))
            {
                layerObject.SetActive(false);
                layerObject.transform.tag = "backgroundSecondary";
                layerObject.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

            } else {
                layerObject.transform.tag = "backgroundPrimary";
            }
        }

    

        List<SVGLayer> maskLayers = svgLayers.Where(layer => layer.file.StartsWith("layer")).ToList();

        foreach (SVGLayer layer in maskLayers)
        {
            UnityEngine.Debug.Log(layer.file);

            string svgAssetPath = Path.Combine(layersPath, layer.file);
            GameObject svgAsset = AssetDatabase.LoadAssetAtPath<GameObject>(svgAssetPath);
            
            GameObject layerObject = SpawnSVGLayer(layersPath, layer);
            SpriteMask mask = layerObject.AddComponent<SpriteMask>();
            mask.sprite = layerObject.GetComponent<SpriteRenderer>().sprite;

            layerObject.AddComponent<SortingGroup>();
            layerObject.AddComponent<Moveable>();          
            MaskLayer maskLayer = layerObject.AddComponent<MaskLayer>();

            // clone the imageObject to be underneath this one\
            foreach(GameObject imageObject in imageObjects)
            {
                GameObject imageObjectClone = Instantiate(imageObject, layerObject.transform);
                imageObjectClone.transform.position = imageObject.transform.position;
                maskLayer.AddRenderer(imageObjectClone);
            }



            // maskLayer.CaptureTexture(Path.Combine("E:/UnityProjects/MarioProject/Assets/", layer.file.Replace(".svg", ".png")));
        }

    }

    private GameObject SpawnSVGLayer(string layersPath, SVGLayer layer)
    {
        string svgAssetPath = Path.Combine(layersPath, layer.file);
        GameObject svgInstance = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(svgAssetPath), layerParent);
        Vector3 targetPos = svgInstance.transform.localPosition;
        targetPos.x += (layer.OffsetVector.x * offsetScalar);
        targetPos.y += (layer.OffsetVector.y * offsetScalar);
        svgInstance.transform.localPosition = targetPos;
        return svgInstance;
    }

    public Texture2D TextureFromSprite()
     {
        Sprite sprite = testSprite;
        Debug.Log("SPRITE " + sprite + " TEXTURE " + sprite.texture);
        return sprite.texture;

        int height = 2886;
        int width = 2899;
        
        Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
        Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
                                                    (int)sprite.textureRect.y, 
                                                    (int)sprite.textureRect.width, 
                                                    (int)sprite.textureRect.height );
        newText.SetPixels(newColors);
        newText.Apply();
        return newText;
        
        // Debug.Log("HERE IS THE SPRITE " + sprite.rect.width + " " + sprite.rect.height + " " + sprite.texture.width + " " + sprite.texture.height);
        //  if(sprite.rect.width != sprite.texture.width){
        //      Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
        //      Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x, 
        //                                                   (int)sprite.textureRect.y, 
        //                                                   (int)sprite.textureRect.width, 
        //                                                   (int)sprite.textureRect.height );
        //      newText.SetPixels(newColors);
        //      newText.Apply();
        //      return newText;
        //  } else
        //      return sprite.texture;
     }
}
#endif