using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OscJack;
using UnityEngine;

// this will be connected to the web socket, and list all the available works
public class ArtworkSceneController : MonoBehaviour, INetworkEndpoint
{
    public string Address => "/scene";

    public Action<ArtworkMetadata> OnArtworkEnabled;
    public Action<ArtworkMetadata> OnArtworkDisabled;

    // public GameObject[] ArtworkPrefabs
    // {
    //     get { return _artworkPrefabs; }
    // }

    [SerializeField]
    private int _maxDisabledArtworks = 3;
    private int _currentlySelectedArtworkIndex = 0;
    public SegmentedPaintingArtwork[] ActiveArtworks => gameObject.GetComponentsInChildren<SegmentedPaintingArtwork>();

    // [SerializeField]
    // private GameObject[] _artworkPrefabs;
    private GameObject[] _activeArtworks;
    public string ResourcePath = "Artworks";
    private readonly string _artworkNamePrefix = "Artwork__";

    // private EndpointHandler _endpointHandler;


    // what if there was an IParameterizedController interface?
    // that exposed a list of all the parameters

    void OnEnable()
    {
        // _endpointHandler = new EndpointHandler(this, "/scene");
        Register("/scene");
    }

    void OnDisable()
    {
        Unregister();
        // _endpointHandler.UnregisterEndpoints();
    }

    public void Unregister()
    {
        if (OscManager.Instance == null)
            return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
    }

    public void Register(string address)
    {
        // make these endpoints fill in controller as the gameObject name
        OscManager.Instance.AddEndpoint(
            $"{address}/toggleArtworkIdx",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsInt(0);
                Debug.Log(
                    $"Toggled Artwork index {value} | number of artworks {ActiveArtworks.Length}"
                );
                ToggleArtworkByIndex(value);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{address}/toggleArtwork",
            (OscDataHandle dataHandle) =>
            {
                Debug.Log("Toggled Artwork");
                var value = dataHandle.GetElementAsString(0);
                Debug.Log($"Toggled Artwork {value} | number of artworks {ActiveArtworks.Length}");
                ToggleArtworkById(value);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{address}/selectArtwork",
            (OscDataHandle dataHandle) =>
            {
                _currentlySelectedArtworkIndex = dataHandle.GetElementAsInt(0);
                Debug.Log($"Selected Artwork {_currentlySelectedArtworkIndex}");
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{address}/enableArtwork",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsString(0);
                Debug.Log($"Enable Artwork {value}");
                EnableArtworkById(value);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{address}/disableArtwork",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsString(0);
                Debug.Log($"Disable Artwork {value}");
                DisableArtworkById(value);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{address}/clear",
            (OscDataHandle dataHandle) =>
            {
                Debug.Log($"Clearing Scene");
                ClearScene();
            },
            this
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
    public void EnableArtworkById(string id)
    {
        string artworkName = $"{_artworkNamePrefix}{id}";
        Debug.Log($"Attempting to enable Artwork {id}");

        // find any existing gameobjects with the same name
        // var artwork = ActiveArtworks.Find(x => x.Id == id);
        var activeArtwork = ActiveArtworks.FirstOrDefault(x => x.gameObject.name == artworkName);

        if (activeArtwork != null)
        {
            Debug.Log($"Artwork {id} already active");
            activeArtwork.CancelDestroy();
            return;
        }

        var artworkPrefab = ArtworkLoader.Instance.GetArtworkPrefab(id);
        if (artworkPrefab == null)
        {
            Debug.Log($"Artwork {id} not found");
            return;
        }

        var newArtwork = Instantiate(artworkPrefab, Vector3.zero, Quaternion.identity);
        newArtwork.transform.parent = transform;
        OnArtworkEnabled?.Invoke(newArtwork.GetComponent<SegmentedPaintingArtwork>().GetMetadata());
    }

    private void DisableArtworkById(string id)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Id == id);
        if (artwork == null)
            return;
        Debug.Log($"Disabling Artwork {artwork.gameObject.name}");
        var info = artwork.GetMetadata();
        artwork.RemoveFromScene(() => OnArtworkDisabled?.Invoke(info));
    }

    public void ToggleArtwork(IArtwork artwork)
    {
        ToggleArtworkById(artwork.Id);
    }

    public void ToggleArtworkById(string id)
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
            artwork.RemoveFromScene();
            // StartCoroutine(ToggleOffArtwork(artwork));
        }
        else
        {
            artwork.gameObject.SetActive(true);
            var colorController = artwork.GetComponent<ArtworkColorController>();
            colorController.FadeInEffect(0, 5);
        }
    }

    private IEnumerator ToggleOffArtwork(SegmentedPaintingArtwork artwork)
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
