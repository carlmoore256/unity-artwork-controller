using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class SceneController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/scene";
    private int _currentlySelectedArtworkId = 0;
    public List<Artwork> Artworks = new List<Artwork>();

    void Start()
    {
        RegisterEndpoints();
        // get all elements in scene with Artwork
        Artworks = FindObjectsOfType<Artwork>(true).ToList();
        Debug.Log("START NUMBER OF ARTWORKS " + Artworks.Count);
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
    }

    int index = 0;

    private void ToggleArtwork(int id) {
        
        var artwork = Artworks.Find(x => x.Id == (int)id);

        if (artwork == null) return;

        Debug.Log($"Toggling Artwork {artwork.Id}");

        // artwork.gameObject.SetActive(!artwork.gameObject.activeSelf);

        if (artwork.gameObject.activeSelf) 
        {
            StartCoroutine(ToggleOffArtwork(artwork.gameObject.GetComponent<ArtworkColorController>()));
        } else {
            var colorController = artwork.gameObject.GetComponent<ArtworkColorController>();
            colorController.gameObject.SetActive(true);
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
