using System;
using OscJack;
using UnityEngine;


// [RequireComponent(typeof(Moveable))]
public class ArtworkColorController : MonoBehaviour, INetworkEndpoint, IArtworkController
{
    public SegmentedPaintingArtwork Artwork => GetComponent<SegmentedPaintingArtwork>();
    public string Address => $"/color";

    // void Start()
    // {
    // }

    void OnEnable()
    {
        // Register();
        ResetToTransparent();
    }

    void OnDisable()
    {
        Unregister();
    }

    public void Unregister()
    {
        if (OscManager.Instance == null)
            return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
        // OscManager.Instance.RemoveEndpoint($"{Address}/opacity");
        // OscManager.Instance.RemoveEndpoint($"{Address}/rotate");
        // OscManager.Instance.RemoveEndpoint($"{Address}/color");
    }

    public void ResetToTransparent()
    {
        Artwork.ForeachMotif(
            (motif) =>
            {
                motif.SetOpacity(0f);
            }
        );
    }

    public void Register(string baseAddress)
    {
        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/color/opacity",
            (OscDataHandle dataHandle) =>
            {
                var fade = dataHandle.GetElementAsFloat(0);
                fade = Mathf.Clamp(fade, 0f, 1f);
                Artwork.ForeachMotif(
                    (motif) =>
                    {
                        motif.SetOpacity(fade);
                    }
                );
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/color/opacityDelayed",
            (OscDataHandle dataHandle) =>
            {
                var fade = dataHandle.GetElementAsFloat(0);
                Artwork.ForeachMotif(
                    (motif, normIndex) =>
                    {
                        motif.SetOpacitySmooth(fade, normIndex);
                    }
                );
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/color/rotate",
            (OscDataHandle dataHandle) =>
            {
                var value = dataHandle.GetElementAsFloat(0);
                Debug.Log($"rotate {value}");
                // make the index influence more when out of phases
                var indexInfluence = ((Mathf.Cos(value * Mathf.PI * 2f) * 0.5f + 0.5f) * -1) + 1f;
                Artwork.ForeachMotif(
                    (motif, normIndex) =>
                    {
                        motif.SetHueOffset(value + (normIndex * indexInfluence));
                    }
                );
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/color/fadeIn",
            (OscDataHandle dataHandle) =>
            {
                FadeInEffect();
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/color/fadeOut",
            (OscDataHandle dataHandle) =>
            {
                FadeOutEffect();
            },
            this
        );
    }

    public void FadeOutEffect(
        float waitTimeLow = 0f,
        float waitTimeHigh = 5f,
        float fadeTimeLow = 0.5f,
        float fadeTimeHigh = 2f,
        Action onComplete = null
    )
    {
        float maxDelay = 0f;
        foreach (var motif in Artwork.Motifs)
        {
            var delay = UnityEngine.Random.Range(waitTimeLow, waitTimeHigh);
            var duration = UnityEngine.Random.Range(fadeTimeLow, fadeTimeHigh);
            maxDelay = Mathf.Max(maxDelay, delay + duration);
            motif.FadeOut(delay, duration);
        }

        CoroutineHelpers.DelayedAction(
            () =>
            {
                onComplete?.Invoke();
            },
            maxDelay,
            this
        );
    }

    public void FadeInEffect(
        float waitTimeLow = 0f,
        float waitTimeHigh = 5f,
        float fadeTimeLow = 0.5f,
        float fadeTimeHigh = 2f
    )
    {
        Artwork.ForeachMotif(
            (motif, normIndex) =>
            {
                var delay = UnityEngine.Random.Range(waitTimeLow, waitTimeHigh);
                var duration = UnityEngine.Random.Range(fadeTimeLow, fadeTimeHigh);
                motif.FadeIn(delay, duration);
            }
        );
    }
}
