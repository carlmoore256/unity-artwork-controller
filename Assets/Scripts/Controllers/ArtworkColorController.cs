using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

// [RequireComponent(typeof(Moveable))]
public class ArtworkColorController : MonoBehaviour, IOscControllable, IArtworkController
{
    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Index}/color";

    void OnEnable()
    {
        RegisterEndpoints();
    }

    void OnDisable()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/opacity");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/rotate");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/color");
    }

    public void RegisterEndpoints()
    {

        OscManager.Instance.AddEndpoint($"{OscAddress}/opacity", (OscDataHandle dataHandle) => {
            var fade = dataHandle.GetElementAsFloat(0);
            Artwork.ForeachMotif((motif) => {
                motif.SetOpacity(fade);
            });
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/opacityDelayed", (OscDataHandle dataHandle) => {
            var fade = dataHandle.GetElementAsFloat(0);
            Artwork.ForeachMotif((motif, normIndex) => {
                motif.SetOpacityDelayed(fade, normIndex);
            });
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/rotate", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsFloat(0);
            Debug.Log($"rotate {value}");
            Artwork.ForeachMotif((motif) => {
                motif.SetHueOffset(value);
            });
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/fadeIn", (OscDataHandle dataHandle) => {
            FadeInEffect();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/fadeOut", (OscDataHandle dataHandle) => {   
            FadeOutEffect();
        });
    }

    public void FadeOutEffect(float waitTimeLow = 0f, float waitTimeHigh = 5f, float fadeTimeLow = 0.5f, float fadeTimeHigh = 2f)
    {
        foreach(var motif in Artwork.Motifs)
        {
            var delay = Random.Range(waitTimeLow, waitTimeHigh);
            var duration = Random.Range(fadeTimeLow, fadeTimeHigh);
            motif.FadeOut(delay, duration);
        }
    }

    public void FadeInEffect(float waitTimeLow = 0f, float waitTimeHigh = 5f, float fadeTimeLow = 0.5f, float fadeTimeHigh = 2f)
    {
        foreach(var motif in Artwork.Motifs)
        {
            var delay = Random.Range(waitTimeLow, waitTimeHigh);
            var duration = Random.Range(fadeTimeLow, fadeTimeHigh);
            motif.FadeIn(delay, duration);
        }
    }
}