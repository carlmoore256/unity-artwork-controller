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

    private int _currentlySelectedArtworkIndex = 0;
    public IArtwork[] ActiveArtworks => gameObject.GetComponentsInChildren<IArtwork>();
    public IEnumerable<IArtwork> AvailableArtworks => ArtworkLoader.Instance.AvailableArtworks;

    private ArtworkTransitionManager _artworkTransitionManager = new ArtworkTransitionManager(
        1f,
        1f
    );

    void Awake()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0; // VSync must be disabled
        Application.targetFrameRate = 60;
#endif
    }

    void OnEnable()
    {
        Register("/scene");
    }

    void OnDisable()
    {
        Unregister();
    }

    public void Unregister()
    {
        if (OscManager.Instance == null)
            return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
    }

    public void Register(string address)
    {
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
            $"{address}/clear",
            (OscDataHandle dataHandle) =>
            {
                Debug.Log($"Clearing Scene");
                ClearScene();
            },
            this
        );
    }

    public bool IsArtworkEnabled(IArtwork artwork)
    {
        return ActiveArtworks.Any(x => x.Id == artwork.Id);
    }

    public void ClearScene()
    {
        foreach (var artwork in ActiveArtworks)
        {
            _artworkTransitionManager.TransitionArtworkOut(artwork, () => { });
        }
    }

    private IArtwork InstantiateArtworkById(string id)
    {
        var artworkPrefab = ArtworkLoader.Instance.GetArtworkPrefab(id);
        if (artworkPrefab == null)
        {
            Debug.Log($"Artwork {id} not found");
            return null;
        }
        var newArtwork = Instantiate(artworkPrefab, Vector3.zero, Quaternion.identity);
        newArtwork.transform.parent = transform;
        var artworkComponent = newArtwork.GetComponent<IArtwork>();
        return artworkComponent;
    }

    public void ToggleArtwork(IArtwork artwork) => ToggleArtworkById(artwork.Id);

    public void ToggleArtworkById(string id)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Id == id);
        if (artwork == null)
        {
            // EnableArtworkById(id); // turn the artwork on
            artwork = InstantiateArtworkById(id);
            if (artwork == null)
                return;
        }

        _artworkTransitionManager.ToggleTransition(
            artwork,
            () =>
            {
                OnArtworkEnabled?.Invoke(artwork.GetMetadata());
                Debug.Log($"Artwork {artwork.Id} toggled on");
            },
            () =>
            {
                OnArtworkDisabled?.Invoke(artwork.GetMetadata());
                Debug.Log($"Artwork {artwork.Id} toggled off");
            },
            (TransitionState state) =>
            {
                Debug.Log($"Artwork {artwork.Id} toggled {state}");
                if (state == TransitionState.In)
                {
                    OnArtworkEnabled?.Invoke(artwork.GetMetadata());
                }
            }
        );
    }
}
