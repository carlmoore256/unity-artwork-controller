using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class ArtworkSceneController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/scene";
    private int _currentlySelectedArtworkIndex = 0;
    public List<Artwork> ActiveArtworks = new List<Artwork>();
    private GameObject[] _artworkPrefabs;
    private GameObject[] _activeArtworks;
    public string ResourcePath = "Artworks";

    void Start()
    {
        _artworkPrefabs = Resources.LoadAll<GameObject>(ResourcePath);
        if (_artworkPrefabs.Length == 0) {
            throw new System.Exception($"No Artwork Prefabs found in Resources/{ResourcePath}");
        }


        foreach(var prefab in _artworkPrefabs)
        {
            Debug.Log("PREFAB " + prefab.name + " Id: " + prefab.GetComponent<Artwork>().Id);
        }

        // ActiveArtworks = _artworkPrefabs.Select(x => x.GetComponent<Artwork>()).ToList();
        
        // // get all elements in scene with Artwork
        // // Artworks = FindObjectsOfType<Artwork>(true).ToList();
        // Debug.Log("START NUMBER OF ARTWORKS " + ActiveArtworks.Count);

        // foreach (var artwork in ActiveArtworks)
        // {
        //     artwork.Index = ActiveArtworks.IndexOf(artwork);
        //     artwork.gameObject.SetActive(false);
        // }

        // RegisterEndpoints();
    }

    void OnEnable()
    {
        RegisterEndpoints();
    }

    void OnDisable()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/toggleArtwork");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/selectArtwork");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableArtwork");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/disableArtwork");
    }

    public void RegisterEndpoints()
    {
        // make these endpoints fill in controller as the gameObject name

        OscManager.Instance.AddEndpoint($"{OscAddress}/toggleArtwork", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsInt(0);
            Debug.Log($"Toggled Artwork {value} | number of artworks {ActiveArtworks.Count}");
            ToggleArtwork(value);
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/selectArtwork", (OscDataHandle dataHandle) => {
            _currentlySelectedArtworkIndex = dataHandle.GetElementAsInt(0);
            Debug.Log($"Selected Artwork {_currentlySelectedArtworkIndex}");
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


    private void EnableArtwork(string id)
    {
        Debug.Log($"Attempting to enable Artwork {id}");
        
        var artwork = ActiveArtworks.Find(x => x.Id == id);
        if (artwork != null) {
            Debug.Log($"Artwork {id} already active");
            return;
        };
        
        var artworkPrefab = _artworkPrefabs.FirstOrDefault(x => x.GetComponent<Artwork>().Id == id);
        if (artworkPrefab == null) {
            Debug.Log($"Artwork {id} not found");
            return;
        }

        var newArtwork = Instantiate(artworkPrefab, Vector3.zero, Quaternion.identity);
        newArtwork.transform.parent = transform;     
        artwork.gameObject.SetActive(true);

        // var colorController = artwork.gameObject.GetComponent<ArtworkColorController>();
        // colorController.FadeInEffect(0, 5);
    }

    // private void SpawnArtwork()


    private void DisableArtwork(string id)
    {

    }

    // private void EnableArtwork(string name)
    // {
    //     // find any artworks after splitting name after __
    //     var artwork = Artworks.Find(x => x.gameObject.name == $"Artwork__{name}");
    //     if (artwork == null) return;
    //     Debug.Log($"Enabling Artwork {artwork.gameObject.name}");
    //     artwork.gameObject.SetActive(true);
    //     var colorController = artwork.gameObject.GetComponent<ArtworkColorController>();
    //     colorController.FadeInEffect(0, 5);
    // }

    // private void DisableArtwork(string name)
    // {
    //     var artwork = Artworks.Find(x => x.gameObject.name == $"Artwork__{name}");
    //     if (artwork == null) return;
    //     Debug.Log($"Disabling Artwork {artwork.gameObject.name}");
    //     StartCoroutine(ToggleOffArtwork(artwork.gameObject.GetComponent<ArtworkColorController>()));
    // }

    private void ToggleArtwork(int index) {
        
        var artwork = ActiveArtworks.Find(x => x.Index == (int)index);

        if (artwork == null) {
            Debug.Log($"Artwork {index} not found");
            return;
        };

        Debug.Log($"Toggling Artwork {artwork.Index}");

        if (artwork.gameObject.activeSelf) 
        {
            StartCoroutine(ToggleOffArtwork(artwork.gameObject.GetComponent<ArtworkColorController>()));
        } else {
            artwork.gameObject.SetActive(true);
            var colorController = artwork.gameObject.GetComponent<ArtworkColorController>();
            colorController.FadeInEffect(0, 5);
        }
    }

    private IEnumerator ToggleOffArtwork(ArtworkColorController colorController)
    {
        colorController.FadeOutEffect(0,2);
        yield return new WaitForSeconds(3f);
        colorController.gameObject.SetActive(false);
    }

    public Vector3 GetArtworkPosition(int index) {
        var artwork = ActiveArtworks.Find(x => x.Index == (int)index);

        if (artwork == null) return Vector3.zero;

        return artwork.transform.position;
    }
}
