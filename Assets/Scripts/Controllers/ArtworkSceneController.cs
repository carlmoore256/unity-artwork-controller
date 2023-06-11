using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class ArtworkSceneController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/scene";

    [SerializeField] private int _maxDisabledArtworks = 3;
    private int _currentlySelectedArtworkIndex = 0;
    public Artwork[] ActiveArtworks => gameObject.GetComponentsInChildren<Artwork>();

    //  = new List<Artwork>();
    private GameObject[] _artworkPrefabs;
    private GameObject[] _activeArtworks;
    public string ResourcePath = "Artworks";
    private readonly string _artworkNamePrefix = "Artwork__";
    

    void Start()
    {
        _artworkPrefabs = Resources.LoadAll<GameObject>(ResourcePath);
        if (_artworkPrefabs.Length == 0) {
            throw new System.Exception($"No Artwork Prefabs found in Resources/{ResourcePath}");
        }
    }

    void OnEnable()
    {
        RegisterEndpoints();

        // ActiveArtworks = new List<Artwork>();
        // GetComponentsInChildren<Artwork>().ToList().ForEach(x => {
        //     ActiveArtworks.Add(x);
        // });

    }

    void OnDisable()
    {
        UnregisterEndpoints();
    }

    public void UnregisterEndpoints()
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

        OscManager.Instance.AddEndpoint($"{OscAddress}/toggleArtworkIdx", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsInt(0);
            Debug.Log($"Toggled Artwork index {value} | number of artworks {ActiveArtworks.Length}");
            ToggleArtworkByIndex(value);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/toggleArtwork", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Toggled Artwork {value} | number of artworks {ActiveArtworks.Length}");
            ToggleArtworkById(value);
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/selectArtwork", (OscDataHandle dataHandle) => {
            _currentlySelectedArtworkIndex = dataHandle.GetElementAsInt(0);
            Debug.Log($"Selected Artwork {_currentlySelectedArtworkIndex}");
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/enableArtwork", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Enable Artwork {value}");
            EnableArtworkById(value);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/disableArtwork", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Disable Artwork {value}");
            DisableArtworkById(value);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/clear", (OscDataHandle dataHandle) => {
            Debug.Log($"Clearing Scene");
            ClearScene();
        });
    }

    public void ClearScene()
    {
        foreach(var artwork in ActiveArtworks)
        {
            artwork.RemoveFromScene();
        }
    }


    private void EnableArtworkById(string id)
    {
        string artworkName = $"{_artworkNamePrefix}{id}";
        Debug.Log($"Attempting to enable Artwork {id}");


        // find any existing gameobjects with the same name
        
        // var artwork = ActiveArtworks.Find(x => x.Id == id);
        var artwork = ActiveArtworks.FirstOrDefault(x => x.gameObject.name == artworkName);
        if (artwork != null) {
            Debug.Log($"Artwork {id} already active");
            artwork.CancelDestroy();
            return;
        };
        
        var artworkPrefab = _artworkPrefabs.FirstOrDefault(x => x.GetComponent<Artwork>().Id == id);
        if (artworkPrefab == null) {
            Debug.Log($"Artwork {id} not found");
            return;
        }

        var newArtwork = Instantiate(artworkPrefab, Vector3.zero, Quaternion.identity);
        newArtwork.transform.parent = transform;
        // newArtwork.gameObject.name = artworkName;
        // newArtwork.GetComponent<Artwork>().AddArtworkControllers();

        // newArtwork.GetComponent<Artwork>().ForeachMotif((motif) => {
        //     motif.GetComponent<MotifColorController>().SetRandomColor();
        // });


        // ActiveArtworks.Add(newArtwork.GetComponent<Artwork>());
        Debug.Log($"Enabling Artwork {newArtwork.name}");
    }


    private void DisableArtworkById(string id)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Id == id);
        if (artwork == null) return;
        Debug.Log($"Disabling Artwork {artwork.gameObject.name}");
        artwork.RemoveFromScene();
    }

    private void ToggleArtworkById(string id)
    {
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Id == id);
        if (artwork == null) {
            EnableArtworkById(id); // turn the artwork on
        } else {
            // if the artwork is pending destruction, turn it back on
            if (artwork.MarkedToDestroy) { 
                artwork.CancelDestroy();
                return;
            }
            DisableArtworkById(id);
        }
    }

    private void ToggleArtworkByIndex(int index) {
        
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Index == (int)index);

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
        var artwork = ActiveArtworks.FirstOrDefault(x => x.Index == (int)index);

        if (artwork == null) return Vector3.zero;

        return artwork.transform.position;
    }
}
