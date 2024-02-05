using System.Collections.Generic;
using UnityEngine.Rendering;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
public class SVGImporter : MonoBehaviour
{
    [Header("Resource Location")]
    public string folderRoot = "SVG";
    public string svgName;

    [Header("Load Parameters")]
    public float offsetScalar = 0.01f;
    public Vector3 bgOffset = new Vector3(0.05f, 0.1f, 0);
    public Vector2 bgScale = new Vector2(1f, 1f);

    public string ArtworkRootPath { get { return Path.Combine("Assets", folderRoot, svgName); } }

    private readonly string _controllerPrefix = "Artwork__";
    private readonly string _layersJSON = "layers.json";
    private List<IArtworkController> _artworkControllers = new List<IArtworkController>();

    public void LoadAndApplySVGs()
    {
        Transform artworkContainer = InitializeArtworkContainer();

        // string layersPath = Path.Combine("Assets", folderRoot, svgName, "Layers");
        string layersPath = Path.Combine(ArtworkRootPath, "Layers");

        // load the configuration file for layers
        List<SVGLayer> svgLayers = LoadSVGLayers(layersPath, _layersJSON);
        if (svgLayers.Count == 0) throw new System.Exception("No SVG offsets found!");

        // create the images that will form the background
        List<GameObject> imageBackgrounds = CreateImageBackgrounds(artworkContainer, layersPath, svgLayers);

        // create the layers that form the whole piece
        CreateLayers(artworkContainer, layersPath, svgLayers, imageBackgrounds);

        // set the background layer to inactive (REMOVE ME)
        var imageOrig = artworkContainer.Find("image-orig");
        if (imageOrig != null) imageOrig.gameObject.SetActive(false);
    }

    private void CreateLayers(Transform artworkContainer, string layersPath, List<SVGLayer> svgLayers, List<GameObject> imageBackgrounds)
    {
        // TODO: SET THE LAYER ORDER
        IEnumerable<SVGLayer> maskLayers = svgLayers.Where(layer => layer.file.StartsWith("layer"));

        foreach (SVGLayer layer in maskLayers)
        {
            string svgAssetPath = Path.Combine(layersPath, layer.file);
            GameObject svgAsset = AssetDatabase.LoadAssetAtPath<GameObject>(svgAssetPath);

            GameObject layerObject = SpawnSVGLayer(layersPath, layer, artworkContainer);
            SpriteMask mask = layerObject.AddComponent<SpriteMask>();
            mask.sprite = layerObject.GetComponent<SpriteRenderer>().sprite;

            layerObject.AddComponent<SortingGroup>();
            layerObject.AddComponent<Moveable>();
            MaskLayer maskLayer = layerObject.AddComponent<MaskLayer>();

            // clone the imageObject to be underneath this one\
            foreach (GameObject imageObject in imageBackgrounds)
            {
                GameObject imageObjectClone = Instantiate(imageObject, layerObject.transform);
                imageObjectClone.transform.position = imageObject.transform.position;
                maskLayer.AddRenderer(imageObjectClone);
            }

            // maskLayer.CaptureTexture(Path.Combine("E:/UnityProjects/MarioProject/Assets/", layer.file.Replace(".svg", ".png")));
        }
    }

    /// <summary>
    /// Create the layers that contain background images within the SVG
    /// </summary>
    private List<GameObject> CreateImageBackgrounds(Transform artworkContainer, string layersPath, List<SVGLayer> svgLayers)
    {
        // find all layers with "image" in name
        IEnumerable<SVGLayer> imageLayers = svgLayers.Where(layer => layer.file.StartsWith("image"));

        List<GameObject> imageObjects = new List<GameObject>();

        foreach (SVGLayer layer in imageLayers)
        {
            GameObject layerObject = SpawnSVGLayer(layersPath, layer, artworkContainer);
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
            }
            else
            {
                layerObject.transform.tag = "backgroundPrimary";
            }
        }
    
        return imageObjects;
    }

    /// <summary>
    /// Parses the layers JSON file for the given layers path, and returns a list of SVGLayers
    /// </summary>
    private List<SVGLayer> LoadSVGLayers(string layersPath, string configJSON = "layers.json")
    {
        string jsonFilePath = Path.Combine(layersPath, configJSON);
        string jsonString = File.ReadAllText(jsonFilePath);
        // Deserialize the JSON data
        List<SVGLayer> svgLayers = JsonConvert.DeserializeObject<List<SVGLayer>>(jsonString);
        return svgLayers;
    }

    /// <summary>
    /// Adds all monobehaviour artwork controllers to the artwork container,
    /// providing control with OSC endpoints
    /// </summary>
    // private static void AddArtworkControllers(Artwork artwork)
    // {
    //     // this didn't work
    //     // Debug.Log("Artwork Controllers " + _artworkControllers.Count);
    //     // foreach(IArtworkController controller in _artworkControllers) {
    //     //     Debug.Log("Adding controller: " + controller.GetType().ToString());
    //     //     artwork.AddController(controller);
    //     // }

    //     // add all the monobehaviours
    //     artwork.gameObject.AddComponent<PolyphonicMidiController>();
    //     artwork.gameObject.AddComponent<ArtworkColorController>();
    //     artwork.gameObject.AddComponent<MotifMotionController>();
    //     artwork.gameObject.AddComponent<LineTrailController>();
    // }


    /// <summary>
    /// Initialize an empty container to place the SVG layers into
    /// </summary>
    private Transform InitializeArtworkContainer()
    {
        // spawn an empty gameobject
        Transform artworkContainer = new GameObject(_controllerPrefix + svgName).transform;
        artworkContainer.SetParent(transform);
        artworkContainer.transform.localPosition = Vector3.zero;
        artworkContainer.gameObject.AddComponent<SegmentedPaintingArtwork>();

        SegmentedPaintingArtwork artwork = artworkContainer.GetComponent<SegmentedPaintingArtwork>();
        artwork.AddArtworkControllers();
        // AddArtworkControllers(artwork);

        // fully refresh the contents of the container, 
        // removing any existing layers in the parent if the user re-generates
        DestroyChildren(artworkContainer);

        return artworkContainer;
    }

    private static void DestroyChildren(Transform artworkContainer)
    {
        List<Transform> childrenToDestroy = new List<Transform>();
        foreach (Transform child in artworkContainer) childrenToDestroy.Add(child);
        foreach (Transform child in childrenToDestroy) DestroyImmediate(child.gameObject);
    }

    private GameObject SpawnSVGLayer(string layersPath, SVGLayer layer, Transform container)
    {
        string svgAssetPath = Path.Combine(layersPath, layer.file);
        GameObject svgInstance = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(svgAssetPath), container);
        Vector3 targetPos = svgInstance.transform.localPosition;
        targetPos.x += (layer.OffsetVector.x * offsetScalar);
        targetPos.y += (layer.OffsetVector.y * offsetScalar);
        svgInstance.transform.localPosition = targetPos;
        return svgInstance;
    }


    # region Editor
    
    public void AddArtworkController(IArtworkController artworkController)
    {
        if (!_artworkControllers.Contains(artworkController))
        {
            _artworkControllers.Add(artworkController);
        }
    }

    public void ClearArtworkControllers()
    {
        _artworkControllers.Clear();
    }

    # endregion
}
#endif