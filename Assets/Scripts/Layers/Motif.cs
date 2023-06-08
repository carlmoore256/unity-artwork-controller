using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a layer or collection of layers that have related properties and
// are an artistic motif
public class Motif : MonoBehaviour
{
    public bool IsLeaf { get {
        return GetComponentsInChildren<Motif>().Length == 0;
    } }

    private MaskLayer[] _maskLayers;

    void Start()
    {
        _maskLayers = GetComponentsInChildren<MaskLayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
