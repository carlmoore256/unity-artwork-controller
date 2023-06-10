using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;


public class Artwork : MonoBehaviour, IComponentIterator, IMaskLayerIterator, IMovableIterator, IMotifIterator
{
    [SerializeField] private int _index;
    public int Index { get => _index; set => _index = value; }

    public string Id => gameObject.name.Split("Artwork__")[1];

    // this is all any controller will ever need to access
    public List<Motif> Motifs = new List<Motif>();


    private List<Moveable> _moveables = new List<Moveable>();
    private List<MaskLayer> _maskLayers = new List<MaskLayer>();

    public IEnumerable<MaskLayer> AllMaskLayers {
        get {
            if (_maskLayers == null) {
                _maskLayers = Motifs.SelectMany(m => m.MaskLayers).ToList();
            }
            return _maskLayers;
        }
    } 

    public IEnumerable<Moveable> AllMoveables {
        get {
            if (_moveables == null) {
                _moveables = Motifs.SelectMany(m => m.Moveables).ToList();
            }
            return _moveables;
        }
    }
    
    // public IEnumerable<Moveable> AllMoveables => Motifs.SelectMany(m => m.Moveables); // call these less as they are more expensive
    

    void Start()
    {
        InitializeMotifs();
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
            Motifs.Add(motif);
        }
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
        if (normalizedIndex > 1) normalizedIndex = 1;
        else if (normalizedIndex < 0) normalizedIndex = 0;
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
        if (normalizedIndex > 1) normalizedIndex = 1;
        else if (normalizedIndex < 0) normalizedIndex = 0;
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
        if (normalizedIndex > 1) normalizedIndex = 1;
        else if (normalizedIndex < 0) normalizedIndex = 0;
        return AllMaskLayers.ElementAt(Mathf.FloorToInt(normalizedIndex * AllMaskLayers.Count()));
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
        if (normalizedIndex > 1) normalizedIndex = 1;
        else if (normalizedIndex < 0) normalizedIndex = 0;
        return AllMoveables.ElementAt(Mathf.FloorToInt(normalizedIndex * AllMoveables.Count()));
    }

    # endregion
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
