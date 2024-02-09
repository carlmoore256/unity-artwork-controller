using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtworkLoader : MonoBehaviour
{
    public static ArtworkLoader Instance { get; private set; }

    [SerializeField]
    private string _resourcePath = "Artworks";

    private Dictionary<string, GameObject> _artworkPrefabs;

    public IEnumerable<IArtwork> AvailableArtworks =>
        _artworkPrefabs.Values.Select(prefab => prefab.GetComponent<IArtwork>());

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            Instance = new GameObject("ArtworkLoader").AddComponent<ArtworkLoader>();
            DontDestroyOnLoad(Instance.gameObject);
            Instance.LoadArtworkPrefabs();
        }
    }

    public IArtwork GetArtwork(string id)
    {
        if (_artworkPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab.GetComponent<IArtwork>();
        }
        return null;
    }

    public GameObject GetArtworkPrefab(string id)
    {
        if (_artworkPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab;
        }
        return null;
    }

    private void LoadArtworkPrefabs()
    {
        var prefabs = Resources.LoadAll<GameObject>(_resourcePath);
        Dictionary<string, GameObject> artworkPrefabs = new Dictionary<string, GameObject>();
        foreach (var prefab in prefabs)
        {
            if (!prefab.TryGetComponent<IArtwork>(out var component))
            {
                Debug.Log($"Artwork Prefab {prefab.name} does not have an Artwork component");
            }
            else
            {
                artworkPrefabs.Add(component.Id, prefab);
            }
        }
        _artworkPrefabs = artworkPrefabs;
        Debug.Log($"Loaded {_artworkPrefabs.Count} artwork prefabs");
    }

    public Texture2D GetArtworkThumbnail(IArtwork artwork)
    {
        return GetArtworkThumbnailFromId(artwork.Id);
    }

    public Texture2D GetArtworkThumbnailFromId(string artworkId)
    {
        string path = $"{_resourcePath}/thumbnails/Artwork__{artworkId}";
        var texture = Resources.Load<Texture2D>(path);
        if (texture == null)
        {
            Debug.LogError($"No thumbnail found at {path}");
            return null;
        }
        // we have to duplicate to get a valid readable texture
        return texture.Duplicate();
    }
}
