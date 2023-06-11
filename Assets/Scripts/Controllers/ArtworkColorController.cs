using UnityEngine;
using OscJack;
using System;

// [RequireComponent(typeof(Moveable))]
public class ArtworkColorController : MonoBehaviour, IOscControllable, IArtworkController
{
    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/color";

    void OnEnable()
    {
        RegisterEndpoints();
    }

    void OnDisable()
    {
        UnregisterEndpoints();
    }
    
    void OnDestroy() {
        UnregisterEndpoints();
    }

    public void UnregisterEndpoints()
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
            fade = Mathf.Clamp(fade, 0f, 1f);
            Artwork.ForeachMotif((motif) => {
                motif.SetOpacity(fade);
            });
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/opacityDelayed", (OscDataHandle dataHandle) => {
            var fade = dataHandle.GetElementAsFloat(0);
            Artwork.ForeachMotif((motif, normIndex) => {
                motif.SetOpacitySmooth(fade, normIndex);
            });
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/rotate", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsFloat(0);
            Debug.Log($"rotate {value}");
            // make the index influence more when out of phases
            var indexInfluence = ((Mathf.Cos(value * Mathf.PI * 2f) * 0.5f + 0.5f) * -1) + 1f;
            Artwork.ForeachMotif((motif, normIndex) => {
                motif.SetHueOffset(value + (normIndex * indexInfluence));
            });
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/fadeIn", (OscDataHandle dataHandle) => {
            FadeInEffect();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/fadeOut", (OscDataHandle dataHandle) => {   
            FadeOutEffect();
        });
    }

    public void FadeOutEffect(float waitTimeLow = 0f, float waitTimeHigh = 5f, float fadeTimeLow = 0.5f, float fadeTimeHigh = 2f, Action onComplete = null)
    {
        float maxDelay = 0f;
        foreach(var motif in Artwork.Motifs)
        {
            var delay = UnityEngine.Random.Range(waitTimeLow, waitTimeHigh);
            var duration = UnityEngine.Random.Range(fadeTimeLow, fadeTimeHigh);
            maxDelay = Mathf.Max(maxDelay, delay + duration);
            motif.FadeOut(delay, duration);
        }

        CoroutineHelpers.DelayedAction(() => {
            onComplete?.Invoke();
        }, maxDelay, this);
    }

    public void FadeInEffect(float waitTimeLow = 0f, float waitTimeHigh = 5f, float fadeTimeLow = 0.5f, float fadeTimeHigh = 2f)
    {
        // foreach(var motif in Artwork.Motifs)
        // {
        //     var delay = UnityEngine.Random.Range(waitTimeLow, waitTimeHigh);
        //     var duration = UnityEngine.Random.Range(fadeTimeLow, fadeTimeHigh);
        //     motif.FadeIn(delay, duration);
        // }
        Artwork.ForeachMotif((motif, normIndex) => {
            var delay = UnityEngine.Random.Range(waitTimeLow, waitTimeHigh);
            var duration = UnityEngine.Random.Range(fadeTimeLow, fadeTimeHigh);
            motif.FadeIn(delay, duration);
        });
    }
}