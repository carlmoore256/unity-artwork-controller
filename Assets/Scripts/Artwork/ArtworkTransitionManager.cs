using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ArtworkTransitionManager
{
    public float inDuration = 4f;
    public float outDuration = 4f;

    public ArtworkTransitionManager(float inDuration, float outDuration)
    {
        this.inDuration = inDuration;
        this.outDuration = outDuration;
    }

    private static ITransitionInsert GetTransitionInsert(IArtwork artwork)
    {
        return artwork.GetInserts().FirstOrDefault(insert => insert is ITransitionInsert)
            as ITransitionInsert;
    }

    public void ToggleTransition(IArtwork artwork, Action onInComplete, Action onOutComplete, Action<TransitionState> onRequestToggle)
    {
        var transitionInsert = GetTransitionInsert(artwork);
        if (transitionInsert == null)
        {
            Debug.LogWarning($"Artwork {artwork.Name} does not have a transition insert");
            return;
        }

        Debug.Log($"WHAT IS THE STATE? {transitionInsert.State}");
        switch (transitionInsert.State)
        {
            case TransitionState.In:
                onRequestToggle?.Invoke(TransitionState.Out);
                TransitionArtworkOut(artwork, onOutComplete);
                break;
            case TransitionState.Out:
                onRequestToggle?.Invoke(TransitionState.In);
                TransitionArtworkIn(artwork, onInComplete);
                break;
            case TransitionState.Idle:
                onRequestToggle?.Invoke(TransitionState.Out);
                TransitionArtworkOut(artwork, onInComplete);
                break;
            case TransitionState.None:
                onRequestToggle?.Invoke(TransitionState.In);
                transitionInsert.Initialize();
                TransitionArtworkIn(artwork, onInComplete);
                break;
        }
    }

    public void TransitionArtworkIn(
        IArtwork artwork,
        Action onComplete = null,
        ITransitionInsert transitionInsert = null
    )
    {
        if (transitionInsert == null)
        {
            transitionInsert = GetTransitionInsert(artwork);
        }

        if (transitionInsert == null)
        {
            Debug.LogWarning($"Artwork {artwork.Name} does not have a transition insert");
            return;
        }

        // in this case, its fading out and we want to cancel that and fade back in
        if (transitionInsert.State == TransitionState.None)
        {
            transitionInsert.Initialize();
        }

        transitionInsert.TransitionIn(inDuration, onComplete);
    }

    public void TransitionArtworkOut(
        IArtwork artwork,
        Action onComplete = null,
        ITransitionInsert transitionInsert = null
    )
    {
        if (transitionInsert == null)
        {
            transitionInsert = GetTransitionInsert(artwork);
        }

        if (transitionInsert == null)
        {
            Debug.LogWarning($"Artwork {artwork.Name} does not have a transition insert");
            return;
        }

        if (transitionInsert.State == TransitionState.Out)
        {
            Debug.Log($"Artwork {artwork.Name} is already transitioning out");
            return;
        }

        transitionInsert.TransitionOut(
            outDuration,
            () =>
            {
                onComplete?.Invoke();
                artwork.Destroy();
            }
        );
    }
}
