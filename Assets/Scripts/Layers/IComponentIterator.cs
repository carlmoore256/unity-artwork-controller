using System;
using UnityEngine;

public interface IComponentIterator
{
    
    /// <summary>
    /// Generic iterator of children components
    /// </summary>
    public void ForeachComponent<T>(Action<T> action) where T : Component;

    /// <summary>
    /// Generic iterator of children components, providing a float for normalized index
    /// </summary>
    public void ForeachComponent<T>(Action<T, float> action) where T : Component;

    /// <summary>
    /// Get a component in the IComponentIterator's children at a normalized index (0f-1f) 
    public T GetComponentAtNormalizedIndex<T>(float normalizedIndex) where T : Component;
}

public interface IMaskLayerIterator
{
    public int NumMaskLayers { get; }
    
    /// <summary>
    /// Apply actions to all LayerMasks
    /// </summary>
    public void ForeachMaskLayer(Action<MaskLayer> action);
    
    /// <summary>
    /// Apply actions to all LayerMasks, providing a float for normalized index
    /// </summary>
    public void ForeachMaskLayer(Action<MaskLayer, float> action);
    public MaskLayer GetMaskLayerAtNormalizedIndex(float normalizedIndex);
}

public interface IMovableIterator
{
    public int NumMoveables { get; }

    /// <summary>
    /// Iterator of moveables
    /// </summary>
    public void ForeachMoveable(Action<Moveable> action);

    /// <summary>
    /// Iterator of moveables, providing a float for normalized index
    /// </summary>
    public void ForeachMoveable(Action<Moveable, float> action);
    public Moveable GetMoveableAtNormalizedIndex(float normalizedIndex);
}

public interface IMotifIterator
{
    public int NumMotifs { get; }
 
    /// <summary>
    /// Iterator of all motifs
    /// </summary>
    public void ForeachMotif(Action<Motif> action);

    /// <summary>
    /// Iterator of all motifs, providing a float for normalized index
    /// </summary>
    public void ForeachMotif(Action<Motif, float> action);
    public Motif GetMotifAtNormalizedIndex(float normalizedIndex);
}