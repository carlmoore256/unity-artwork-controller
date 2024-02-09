using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// a layer or collection of layers that have related properties and
// are an artistic motif


// idea, the controllers will look for the motif, not the moveable
// the motif will provide the apis for the moveable. That way we can
// independently control motifs, and not all the moveables.
// This should also reduce processing power
public class Motif : MonoBehaviour, IComponentIterator, IMaskLayerIterator, IMovableIterator, IFadable
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
        _responseCurve = UnityEngine.Random.Range(0.5f, 2.5f);
    }

    void OnEnable()
    {
        MaskLayers = GetComponentsInChildren<MaskLayer>();
        Moveables = GetComponentsInChildren<Moveable>();

        // SetOpacity(0f, false);
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

    /// <summary>
    /// Return an average of all the layer colors
    /// </summary>
    public Color GetColor()
    {
        if (MaskLayers.Length == 1) return MaskLayers[0].GetColor();
        Color color = Color.black;
        foreach (var mask in MaskLayers)
        {
            color += mask.GetColor();
        }
        return color / MaskLayers.Length;
    }


    /// <summary>
    /// Set the opacity of all mask layers
    /// </summary>
    public void SetOpacity(float opacity)
    {
        // if (useCurve)
        if (opacity != 0f) 
            opacity = Mathf.Pow(opacity, _responseCurve);
        foreach (var mask in MaskLayers)
        {
            mask.SetOpacity(opacity);
        }
    }

    public void SetOpacitySmooth(float opacity, float delay, float duration = 0.5f, bool useCurve = true)
    {
        if (useCurve) opacity = Mathf.Pow(opacity, _responseCurve);
        if (_opacityCoroutine != null) StopCoroutine(_opacityCoroutine);
        _opacityCoroutine = StartCoroutine(OpacityCoroutine(opacity, duration, delay));
    }


    /// <summary>
    /// Get the average opacity of all mask layers
    /// </summary>
    public float GetOpacity()
    {
        if (MaskLayers.Length == 1) return MaskLayers[0].CurrentOpacity;
        float opacity = 0f;
        foreach (var mask in MaskLayers)
        {
            opacity += mask.CurrentOpacity;
        }
        return opacity / MaskLayers.Length;
    }

    public Vector3 GetPosition()
    {
        if (Moveables.Length == 1) return Moveables[0].transform.position;
        Vector3 position = Vector3.zero;
        foreach (var moveable in Moveables)
        {
            position += moveable.transform.position;
        }
        return position / Moveables.Length;
    }


    /// <summary>
    /// Set the hue offset of all mask layers
    /// </summary>
    public void SetHueOffset(float offset)
    {
        foreach (var mask in MaskLayers)
        {
            mask.SetHueOffset(offset);
        }
    }

    public void FadeOut(float duration, float delay = 0f)
    {
        if (_opacityCoroutine != null) StopCoroutine(_opacityCoroutine);
        _opacityCoroutine = StartCoroutine(OpacityCoroutine(0f, duration, delay));
    }

    public void FadeIn(float duration, float delay = 0f)
    {
        if (_opacityCoroutine != null) StopCoroutine(_opacityCoroutine);
        _opacityCoroutine = StartCoroutine(OpacityCoroutine(1f, duration, delay));
    }

    public void CancelFade()
    {
        if (_opacityCoroutine != null) StopCoroutine(_opacityCoroutine);
    }

    private Coroutine _opacityCoroutine;

    /// <summary>
    /// Sets the opacity of the entire motif
    /// </summary>
    private IEnumerator OpacityCoroutine(float targetOpacity, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        float time = 0f;
        float startOpacity = GetOpacity(); // get the average opacity
        while (time < duration)
        {
            time += Time.deltaTime;
            SetOpacity(Mathf.Lerp(startOpacity, targetOpacity, time / duration)); // set the average opacity
            yield return null;
        }
        SetOpacity(targetOpacity);
    }

    /// <summary>
    /// Sets the opacity of each individual piece
    /// </summary>
    private IEnumerator PiecewiseOpacityCoroutine(float targetOpacity, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        float t = 0f;
        IEnumerable<float> opacities = MaskLayers.Select(layer => layer.CurrentOpacity);
        while (t < duration)
        {
            for (int i = 0; i < MaskLayers.Length; i++)
            {
                MaskLayers[i].SetOpacity(Mathf.Lerp(opacities.ElementAt(i), targetOpacity, t / duration));
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    private Coroutine _colorCoroutine;
    private IEnumerator ColorCoroutine(Color targetColor, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        float time = 0f;
        Color startColor = GetColor();
        while (time < duration)
        {
            SetColor(Color.Lerp(startColor, targetColor, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        SetColor(targetColor);
    }

    /// maybe remove me, this is handled by artwork
    public List<Motif> GetNearbyMotifs(int maxNum, float distanceThresh = 1f) 
    {
        List<Motif> nearbyMotifs = new List<Motif>();
        var allMotifs = FindObjectsOfType<Motif>();
        if (allMotifs.Length <= 1) return nearbyMotifs;

        // sort by distance
        var sortedMotifs = allMotifs.OrderBy(motif => Vector3.Distance(motif.GetPosition(), GetPosition()));
        
        foreach (var motif in sortedMotifs)
        {
            if (motif == this) continue;
            float distance = Vector3.Distance(motif.GetPosition(), GetPosition());
            if (distance > distanceThresh) break;
            nearbyMotifs.Add(motif);
            if (nearbyMotifs.Count >= maxNum) break;
        }
        return nearbyMotifs;
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
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
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
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        return MaskLayers[Mathf.FloorToInt(normalizedIndex * MaskLayers.Length)];
    }

    
    # endregion

    

    # region IMoveableIterator

    public int NumMoveables => Moveables.Length;

    public void ForeachMoveable(Action<Moveable> action)
    {
        if (Moveables == null) {
            Debug.LogError("Moveables is null", gameObject);
            return;
        };
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
        normalizedIndex = Mathf.Clamp01(normalizedIndex);
        return Moveables[Mathf.FloorToInt(normalizedIndex * Moveables.Length)];
    }


    # endregion

}
