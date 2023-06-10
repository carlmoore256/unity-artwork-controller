using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(ArtworkColorController))]
public class Artwork : MonoBehaviour, IComponentIterator, IMaskLayerIterator, IMovableIterator, IMotifIterator
{
    [SerializeField] private int _index;
    public int Index { get => _index; set => _index = value; }

    public string Id => gameObject.name.Split("Artwork__")[1];

    // this is all any controller will ever need to access
    public List<Motif> Motifs = new List<Motif>();
    public bool MarkedToDestroy = false;


    private List<Moveable> _allMoveables = new List<Moveable>();
    private List<MaskLayer> _allMaskLayers = new List<MaskLayer>();

    public IEnumerable<MaskLayer> AllMaskLayers => _allMaskLayers;

    public IEnumerable<Moveable> AllMoveables => _allMoveables;
    
    // public IEnumerable<Moveable> AllMoveables => Motifs.SelectMany(m => m.Moveables); // call these less as they are more expensive


    private void Start()
    {
        InitializeMotifs();
        if (!gameObject.name.Contains("Artwork__")) {
            gameObject.name = $"Artwork__{gameObject.name}";
        }
    }


    private void OnEnable() => FadeIn();

    private void FadeIn()
    {
        var colorController = GetComponent<ArtworkColorController>();
        if (colorController == null) {
            colorController = gameObject.AddComponent<ArtworkColorController>();
        }
        colorController.FadeInEffect();
    }

    public void RemoveFromScene(Action onComplete = null)
    {
        // destroys after things have faded out
        var colorController = GetComponent<ArtworkColorController>();
        if (colorController == null) {
            colorController = gameObject.AddComponent<ArtworkColorController>();
        }
        MarkedToDestroy = true;
        colorController.FadeOutEffect(onComplete: () => {
            onComplete?.Invoke();
            FinalizeDestroy();
        });
    }

    private void FinalizeDestroy()
    {
        if (!MarkedToDestroy) {
            Debug.Log("Artwork not marked to destroy, cancelling");
            return;
        };
        Debug.Log($"Destroying Artwork: {gameObject.name}");
        Destroy(gameObject);
    }

    public void CancelDestroy()
    {
        if (!MarkedToDestroy) return;
        MarkedToDestroy = false;
        Debug.Log($"Cancelling destroy for Artwork: {gameObject.name}");
        FadeIn();
    }


    /// <summary>
    /// Add motif monobehaviours on to all direct descendents
    /// </summary>
    private void InitializeMotifs()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.CompareTag("backgroundPrimary") || child.gameObject.CompareTag("backgroundSecondary")) {
                Debug.Log("Skipping background", child.gameObject);
                continue;
            };

            if (child.gameObject.GetComponentsInChildren<MaskLayer>().Length == 0) {
                Debug.Log("Skipping non-mask layer", child.gameObject);
                continue;
            }
            var motif = child.GetComponent<Motif>();
            if (motif == null) motif = child.gameObject.AddComponent<Motif>();
            if (!child.gameObject.activeSelf) continue;
            Motifs.Add(motif);
        }

        _allMoveables = Motifs.SelectMany(m => m.Moveables).ToList();
        _allMaskLayers = Motifs.SelectMany(m => m.MaskLayers).ToList();
    }

    public List<Motif> GetNearbyMotifs(Motif motif, int maxNum, float distanceThresh)
    {
        var nearbyMotifs = new List<Motif>();
        foreach (var otherMotif in Motifs)
        {
            if (otherMotif == motif) continue;
            if (Vector3.Distance(motif.transform.position, otherMotif.transform.position) < distanceThresh)
            {
                nearbyMotifs.Add(otherMotif);
                if (nearbyMotifs.Count >= maxNum) break;
            }
        }
        return nearbyMotifs;
    }

    /// Preferred API for interacting with pieces of the artwork, since it allows
    /// motif objects to determine what happens with sub-pieces

    # region IMotifIterator

    public int NumMotifs => Motifs.Count;

    public void ForeachMotif(Action<Motif> action)
    {
        foreach (var motif in Motifs)
        {
            action(motif);
        }
    }

    public void ForeachMotif(Action<Motif, float> action)
    {
        for (int i = 0; i < Motifs.Count; i++)
        {
            action(Motifs[i], (float)i / Motifs.Count);
        }
    }

    public Motif GetMotifAtNormalizedIndex(float normalizedIndex)
    {
        if (Motifs.Count == 0) return null;
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        Debug.Log("normalizedIndex" + normalizedIndex + " Number of Motifs " + Motifs.Count);
        var index = Mathf.FloorToInt(normalizedIndex * Motifs.Count);
        if (index == Motifs.Count) index = Motifs.Count - 1;
        if (index < 0) index = 0;
        return Motifs.ElementAt(Mathf.FloorToInt(normalizedIndex * Motifs.Count));
    }

    # endregion

    /// Call these iterators if you want to get fine grained control over single
    /// pieces within the artwork, rather than delegating a task to a motif

    # region IComponentIterator

    public void ForeachComponent<T>(Action<T> action) where T : Component
    {
        foreach (var motif in Motifs)
        {
            foreach (var component in motif.GetComponentsInChildren<T>())
            {
                action(component);
            }
        }
    }

    public void ForeachComponent<T>(Action<T, float> action) where T : Component
    {
        var components = gameObject.GetComponentsInChildren<T>();
        for (int i = 0; i < components.Length; i++)
        {
            action(components[i], i / components.Length);
        }
    }

    public T GetComponentAtNormalizedIndex<T>(float normalizedIndex) where T : Component
    {
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        var index = Mathf.FloorToInt(normalizedIndex * Motifs.Count);
        var components = GetComponentsInChildren<T>();
        if (components.Count() == 0) return null;
        if (index == Motifs.Count) index = components.Count() - 1;
        if (index < 0) index = 0;
        return GetComponentsInChildren<T>()[Mathf.FloorToInt(normalizedIndex * GetComponentsInChildren<T>().Length)];
    }

    # endregion

    # region IMaskLayerIterator

    public int NumMaskLayers => Motifs.Sum(m => m.NumMaskLayers);

    public void ForeachMaskLayer(Action<MaskLayer> action)
    {
        foreach (var motif in Motifs)
        {
            motif.ForeachMaskLayer(action);
        }
    }

    public void ForeachMaskLayer(Action<MaskLayer, float> action)
    {
        var totalLayers = NumMaskLayers;
        int currentIndex = 0;
        foreach (var motif in Motifs)
        {
            foreach (var layer in motif.MaskLayers)
            {
                action(layer, (float)currentIndex / totalLayers);
                currentIndex++;
            }
        }
    }

    public MaskLayer GetMaskLayerAtNormalizedIndex(float normalizedIndex)
    {
        if (AllMaskLayers.Count() == 0) return null;
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        var index = Mathf.FloorToInt(normalizedIndex * AllMaskLayers.Count());
        if (index == AllMaskLayers.Count()) index = AllMaskLayers.Count() - 1;
        return AllMaskLayers.ElementAt(index);
    }

    # endregion    

    # region IMovableIterator

    public int NumMoveables => Motifs.Sum(m => m.NumMoveables);

    public void ForeachMoveable(Action<Moveable> action)
    {
        foreach (var motif in Motifs)
        {
            motif.ForeachMoveable(action);
        }
    }

    public void ForeachMoveable(Action<Moveable, float> action)
    {
        var totalMoveables = NumMoveables;
        int currentIndex = 0;
        foreach (var motif in Motifs)
        {
            foreach (var moveable in motif.Moveables)
            {
                action(moveable, (float)currentIndex / totalMoveables);
                currentIndex++;
            }
        }
    }

    public Moveable GetMoveableAtNormalizedIndex(float normalizedIndex)
    {
        if (AllMoveables.Count() == 0) return null;
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        var index = Mathf.FloorToInt(normalizedIndex * AllMoveables.Count());
        if (index == AllMoveables.Count()) index = AllMoveables.Count() - 1;
        if (index < 0) index = 0;
        return AllMoveables.ElementAt(index);
    }

    # endregion

    
    public void AddArtworkControllers()
    {
        // this didn't work
        // Debug.Log("Artwork Controllers " + _artworkControllers.Count);
        // foreach(IArtworkController controller in _artworkControllers) {
        //     Debug.Log("Adding controller: " + controller.GetType().ToString());
        //     artwork.AddController(controller);
        // }

        // add all the monobehaviours
        if (gameObject.GetComponent<PolyphonicMidiController>() == null)
            gameObject.AddComponent<PolyphonicMidiController>();

        if (gameObject.GetComponent<ArtworkColorController>() == null)
            gameObject.AddComponent<ArtworkColorController>();

        if (gameObject.GetComponent<MotifMotionController>() == null)
            gameObject.AddComponent<MotifMotionController>();

        if (gameObject.GetComponent<LineTrailController>() == null)
            gameObject.AddComponent<LineTrailController>();
        
        if (gameObject.GetComponent<SpritePhysicsController>() == null)
            gameObject.AddComponent<SpritePhysicsController>();
    }
}






// public GameObject GetObjectAtNormalizedIndex(float normalizedIndex)
// {
//     normalizedIndex *= transform.childCount;
//     var index = Mathf.FloorToInt(normalizedIndex);
//     var child = transform.GetChild(index);
//     return child.gameObject;
// }

// public Motif GetMotifAtNormalizedIndex(float normalizedIndex)
// {
//     normalizedIndex *= Motifs.Count;
//     var index = Mathf.FloorToInt(normalizedIndex);
//     return Motifs[index];
// }
