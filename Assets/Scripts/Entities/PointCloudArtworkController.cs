using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[RequireComponent(typeof(ArtworkColorController))]
public class PointCloudArtworkController : SegmentedPaintingArtwork
{
    [SerializeField] private int _index;


    // public void FadeIn()
    // {
    //     var colorController = GetComponent<ArtworkColorController>();
    //     if (colorController == null) {
    //         colorController = gameObject.AddComponent<ArtworkColorController>();
    //     }
    //     colorController.FadeInEffect();
    // }

    // public void FadeOut()
    // {
    //     var colorController = GetComponent<ArtworkColorController>();
    //     if (colorController == null) {
    //         colorController = gameObject.AddComponent<ArtworkColorController>();
    //     }
    //     colorController.FadeOutEffect();
    // }


    /// <summary>
    /// Add motif monobehaviours on to all direct descendents
    /// </summary>
}