using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a layer or collection of layers that have related properties and
// are an artistic motif


// idea, the controllers will look for the motif, not the moveable
// the motif will provide the apis for the moveable. That way we can
// independently control motifs, and not all the moveables.
// This should also reduce processing power
public class Motif : MonoBehaviour
{
    private float _responseCurve; // a randomly assigned curve that differentiates the motif
    // from others as it responds to commands

    public Moveable Moveable { get { return GetComponent<Moveable>(); } }

    public bool IsLeaf { get {
        return GetComponentsInChildren<Motif>().Length == 0;
    } }

    private MaskLayer[] _maskLayers;

    void Start()
    {
        _maskLayers = GetComponentsInChildren<MaskLayer>();
        _responseCurve = Random.Range(0.5f, 2.5f);
    }

    void OnEnable()
    {
        if (GetComponent<Moveable>() == null)
        {
            gameObject.AddComponent<Moveable>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        foreach (var mask in _maskLayers)
        {
            mask.SetColor(color);
        }
    }
}
