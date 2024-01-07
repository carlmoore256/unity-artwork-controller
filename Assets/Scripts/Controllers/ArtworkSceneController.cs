using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OscJack;
using UnityEngine;

// this will be connected to the web socket, and list all the available works
public class ArtworkSceneController : MonoBehaviour, INetworkEndpoint
{
    public string Address => "/scene";

    [SerializeField]
    private int _maxDisabledArtworks = 3;
    private int _currentlySelectedArtworkIndex = 0;
    public Artwork[] ActiveArtworks => gameObject.GetComponentsInChildren<Artwork>();

    private GameObject[] _artworkPrefabs;
    private GameObject[] _activeArtworks;
    public string ResourcePath = "Artworks";
    private readonly string _artworkNamePrefix = "Artwork__";

    private EndpointHandler _endpointHandler;

    void Start()
    {
        _artworkPrefabs = Resources.LoadAll<GameObject>(ResourcePath);
        if (_artworkPrefabs.Length == 0)
        {
            throw new System.Exception($"No Artwork Prefabs found in Resources/{ResourcePath}");
        }
    }

    void OnEnable()
    {
        // _endpointHandler = new EndpointHandler(this, "/scene");
        RegisterEndpoints();
    }

    void OnDisable()
    {
        _endpointHandler.UnregisterEndpoints();
    }

    void OnDestroy()
    {
        _endpointHandler.UnregisterEndpoints();
    }

    public void UnregisterEndpoints()
    {
        if (OscManager.Instance == null)
            return;
        OscManager.Instance.RemoveEndpoint($"{Address}/toggleArtwork");
        OscManager.Instance.RemoveEndpoint($"{Address}/selectArtwork");
        OscManager.Instance.RemoveEndpoint($"{Address}/enableArtwork");
        OscManager.Instance.RemoveEndpoint($"{Address}/disableArtwork");
    }

    public void RegisterEndpoints()
    {
        // make these endpoints fill in controller as the gameObject name

        OscManager.Instance.AddEndpoint(
            $"{Address}/toggleArtworkIdx",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsInt(0);
                Debug.Log(
                    $"Toggled Artwork index {value} | number of artworks {ActiveArtworks.Length}"
                );
                ToggleArtworkByIndex(value);
            }
        );

        OscManager.Instance.AddEndpoint(
            $"{Address}/toggleArtwork",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsString(0);
                Debug.Log($"Toggled Artwork {value} | number of artworks {ActiveArtworks.Length}");
                ToggleArtworkById(value);
            }
        );

        OscManager.Instance.AddEndpoint(
            $"{Address}/selectArtwork",
            (OscDataHandle dataHandle) =>
            {
                _currentlySelectedArtworkIndex = dataHandle.GetElementAsInt(0);
                Debug.Log($"Selected Artwork {_currentlySelectedArtworkIndex}");
            }
        );

        OscManager.Instance.AddEndpoint(
            $"{Address}/enableArtwork",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsString(0);
                Debug.Log($"Enable Artwork {value}");
                EnableArtworkById(value);
            }
        );

        OscManager.Instance.AddEndpoint(
            $"{Address}/disableArtwork",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsString(0);
                Debug.Log($"Disable Artwork {value}");
                DisableArtworkById(value);
            }
        );

        OscManager.Instance.AddEndpoint(
            $"{Address}/clear",
            (OscDataHandle dataHandle) =>
            {
                Debug.Log($"Clearing Scene");
                ClearScene();
            }
        );
    }

    public void ClearScene()
    {
        foreach (var artwork in ActiveArtworks)
        {
            artwork.RemoveFromScene();
        }
    }

    // [NetworkEndpoint("/enableArtwork")]
    private void EnableArtworkById(string id)
    {
        string artworkName = $"{_artworkNamePrefix}{id}";
        Debug.Log($"Attempting to enable Artwork {id}");

        // find any existing gameobjects with the same name

        // var artwork = ActiveArtworks.Find(x => x.Id == id);
        var artwork = ActiveArtworks.FirstOrDefault(x => x.gameObject.name == artworkName);
        if (artwork != null)
        {
            Debug.Log($"Artwork {id} already active");
            artwork.CancelDestroy();
            return;
        }
        ;

        var artworkPrefab = _artworkPrefabs.FirstOrDefault(x => x.GetComponent<Artwork>().Id == id);
        if (artworkPrefab == null)
        {
            Debug.Log($"Artwork {id} not found");
            return;
        }

        var newArtwork = Instantiate(artworkPrefab, Vector3.zero, Quaternion.identity);
        newArtwork.transform.parent = transform;
    }

    private void DisableArtworkById(string id)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Id == id);
        if (artwork == null)
            return;
        Debug.Log($"Disabling Artwork {artwork.gameObject.name}");
        artwork.RemoveFromScene();
    }

    private void ToggleArtworkById(string id)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Id == id);
        if (artwork == null)
        {
            EnableArtworkById(id); // turn the artwork on
        }
        else
        {
            // if the artwork is pending destruction, turn it back on
            if (artwork.MarkedToDestroy)
            {
                artwork.CancelDestroy();
                return;
            }
            DisableArtworkById(id);
        }
    }

    private void ToggleArtworkByIndex(int index)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Index == (int)index);

        if (artwork == null)
        {
            Debug.Log($"Artwork {index} not found");
            return;
        }

        Debug.Log($"Toggling Artwork {artwork.Index}");

        if (artwork.gameObject.activeSelf)
        {
            StartCoroutine(ToggleOffArtwork(artwork));
        }
        else
        {
            artwork.gameObject.SetActive(true);
            var colorController = artwork.GetComponent<ArtworkColorController>();
            colorController.FadeInEffect(0, 5);
        }
    }

    private IEnumerator ToggleOffArtwork(Artwork artwork)
    {
        artwork.GetComponent<ArtworkColorController>().FadeOutEffect(0, 3);
        yield return new WaitForSeconds(3.3f);
        if (!artwork.MarkedToDestroy)
        {
            artwork.CancelDestroy();
        }
        else
        {
            artwork.gameObject.SetActive(false);
        }
    }

    public Vector3 GetArtworkPosition(int index)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Index == (int)index);

        if (artwork == null)
            return Vector3.zero;

        return artwork.transform.position;
    }
}
