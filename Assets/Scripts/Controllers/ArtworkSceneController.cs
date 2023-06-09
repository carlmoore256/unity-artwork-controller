using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class ArtworkSceneController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/scene";
    private int _currentlySelectedArtworkId = 0;
    public List<Artwork> Artworks = new List<Artwork>();
    private GameObject[] _artworkPrefabs;
    public string ResourcePath = "Artworks";

    void Start()
    {
        _artworkPrefabs = Resources.LoadAll<GameObject>(ResourcePath);
        if (_artworkPrefabs.Length == 0) {
            throw new System.Exception($"No Artwork Prefabs found in Resources/{ResourcePath}");
        }
        
        // get all elements in scene with Artwork
        Artworks = FindObjectsOfType<Artwork>(true).ToList();
        Debug.Log("START NUMBER OF ARTWORKS " + Artworks.Count);

        foreach (var artwork in Artworks)
        {
            artwork.Id = Artworks.IndexOf(artwork);
            artwork.gameObject.SetActive(false);
        }


        RegisterEndpoints();
    }

    public void RegisterEndpoints()
    {
        // make these endpoints fill in controller as the gameObject name

        OscManager.Instance.AddEndpoint($"{OscAddress}/toggleArtwork", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsInt(0);
            Debug.Log($"Toggled Artwork {value} | number of artworks {Artworks.Count}");
            ToggleArtwork(value);
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/selectArtwork", (OscDataHandle dataHandle) => {
            _currentlySelectedArtworkId = dataHandle.GetElementAsInt(0);
            Debug.Log($"Selected Artwork {_currentlySelectedArtworkId}");
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/enableArtwork", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Enable Artwork {value}");
            EnableArtwork(value);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/disableArtwork", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Disable Artwork {value}");
            DisableArtwork(value);
        });
    }

    int index = 0;

    private void EnableArtwork(string name)
    {
        // find any artworks after splitting name after __
        var artwork = Artworks.Find(x => x.gameObject.name == $"Artwork__{name}");
        if (artwork == null) return;
        Debug.Log($"Enabling Artwork {artwork.gameObject.name}");
        artwork.gameObject.SetActive(true);
        var colorController = artwork.gameObject.GetComponent<ArtworkColorController>();
        colorController.FadeInEffect(0, 5);
    }

    private void DisableArtwork(string name)
    {
        var artwork = Artworks.Find(x => x.gameObject.name == $"Artwork__{name}");
        if (artwork == null) return;
        Debug.Log($"Disabling Artwork {artwork.gameObject.name}");
        StartCoroutine(ToggleOffArtwork(artwork.gameObject.GetComponent<ArtworkColorController>()));

        // var colorController = artwork.gameObject.GetComponent<ArtworkColorController>();
        // colorController.FadeOutEffect(0, 5);
    }

    private void ToggleArtwork(int id) {
        
        var artwork = Artworks.Find(x => x.Id == (int)id);

        if (artwork == null) return;

        Debug.Log($"Toggling Artwork {artwork.Id}");

        // artwork.gameObject.SetActive(!artwork.gameObject.activeSelf);

        if (artwork.gameObject.activeSelf) 
        {
            StartCoroutine(ToggleOffArtwork(artwork.gameObject.GetComponent<ArtworkColorController>()));
        } else {
            artwork.gameObject.SetActive(true);
            var colorController = artwork.gameObject.GetComponent<ArtworkColorController>();
            // colorController.gameObject.SetActive(true);
            colorController.FadeInEffect(0, 5);
        }

        // if (artwork.gameObject.activeSelf) {
        //     FadeInArtwork(artwork);
        // } else {
        //     FadeOutArtwork(artwork);
        // }
        // if (artwork.gameObject.GetComponent<ArtworkColorController>() == null) {
        //     Debug.Log("Adding ArtworkColorController");
        //     artwork.gameObject.AddComponent<ArtworkColorController>();
        // }

        // artwork.gameObject.GetComponent<ArtworkColorController>().FadeInEffect(artwork);
    }

    private IEnumerator ToggleOffArtwork(ArtworkColorController colorController)
    {
        colorController.FadeOutEffect(0,2);
        yield return new WaitForSeconds(3f);
        colorController.gameObject.SetActive(false);
    }

    public Vector3 GetArtworkPosition(int id) {
        var artwork = Artworks.Find(x => x.Id == (int)id);

        if (artwork == null) return Vector3.zero;

        return artwork.transform.position;
    }
}
