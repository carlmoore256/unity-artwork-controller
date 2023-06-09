using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// a layer or collection of layers that have related properties and
// are an artistic motif


// idea, the controllers will look for the motif, not the moveable
// the motif will provide the apis for the moveable. That way we can
// independently control motifs, and not all the moveables.
// This should also reduce processing power
public class Motif : MonoBehaviour, IComponentIterator, IMaskLayerIterator, IMovableIterator
{
    public Moveable[] Moveables { get; private set; }
    public MaskLayer[] MaskLayers { get; private set; }



    // public Moveable PrimaryMoveable { get; private set; } <- I don't like this, I don't think we 
    // should stack moveables. Only have them on leaf nodes
    
    public bool IsLeaf { get {
        return GetComponentsInChildren<Moveable>().Length > 1;
    } }

    private float _responseCurve; // a randomly assigned curve that differentiates the motif
    // from others as it responds to commands

    void Start()
    {
        MaskLayers = GetComponentsInChildren<MaskLayer>();
        _responseCurve = UnityEngine.Random.Range(0.5f, 2.5f);
    }

    void OnEnable()
    {
        Moveables = GetComponentsInChildren<Moveable>();
    }

    /// <summary>
    /// Set color of all mask layers
    /// </summary>
    public void SetColor(Color color)
    {
        foreach (var mask in MaskLayers)
        {
            mask.SetColor(color);
        }
    }



    # region IComponentIterator

    public void ForeachComponent<T>(Action<T> action) where T : Component
    {
        foreach(var component in GetComponentsInChildren<T>()) {
            action(component);
        }
    }

    public void ForeachComponent<T>(Action<T, float> action) where T : Component
    {
        T[] components = GetComponentsInChildren<T>();

        for (int i = 0; i < components.Length; i++)
        {
            action(components[i], i/components.Length);
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
    
    public int NumMaskLayers => MaskLayers.Length;

    public void ForeachMaskLayer(Action<MaskLayer> action)
    {
        foreach (var layerMask in MaskLayers)
        {
            action(layerMask);
        }
    }


    public void ForeachMaskLayer(Action<MaskLayer, float> action)
    {
        for (int i = 0; i < MaskLayers.Length; i++)
        {
            action(MaskLayers[i], i/MaskLayers.Length);
        }
    }

    public MaskLayer GetMaskLayerAtNormalizedIndex(float normalizedIndex)
    {
        if (normalizedIndex > 1) normalizedIndex = 1;
        else if (normalizedIndex < 0) normalizedIndex = 0;
        return MaskLayers[Mathf.FloorToInt(normalizedIndex * MaskLayers.Length)];
    }

    
    # endregion

    

    # region IMoveableIterator

    public int NumMoveables => Moveables.Length;

    public void ForeachMoveable(Action<Moveable> action)
    {
        foreach (var moveable in Moveables)
        {
            action(moveable);
        }
    }

    public void ForeachMoveable(Action<Moveable, float> action)
    {
        for (int i = 0; i < Moveables.Length; i++)
        {
            action(Moveables[i], i/Moveables.Length);
        }
    }

    public Moveable GetMoveableAtNormalizedIndex(float normalizedIndex)
    {
        if (normalizedIndex > 1) normalizedIndex = 1;
        else if (normalizedIndex < 0) normalizedIndex = 0;
        return Moveables[Mathf.FloorToInt(normalizedIndex * Moveables.Length)];
    }


    # endregion

}
