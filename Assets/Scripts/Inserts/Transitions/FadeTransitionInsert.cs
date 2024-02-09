using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FadeTransitionInsertParameters : InsertParameters
{
    public RangedParameterValue fadeInDelayRange = new(3f, 0f, 10f, "Fade In Delay Range (s)");
    public RangedParameterValue fadeInDurationRange =
        new(3f, 0f, 10f, "Fade In Duration Range (s)");
    public RangedParameterValue fadeOutDelayRange = new(3f, 0f, 10f, "Fade Out Delay Range (s)");
    public RangedParameterValue fadeOutDurationRange =
        new(3f, 0f, 10f, "Fade Out Duration Range (s)");
    public TriggerParameterValue fadeIn = new("Fade In");
    public TriggerParameterValue fadeOut = new("Fade Out");
}

public class FadeTransitionInsert : MonoBehaviourWithId, ITransitionInsert
{
    public string Name => "Fade Transition";
    public float PercentComplete { get; private set; } = 0f;
    public TransitionState State { get; private set; } = TransitionState.None;

    private FadeTransitionInsertParameters _parameters = new FadeTransitionInsertParameters();

    private IFadable[] _fadableComponents;

    private Coroutine _currentTransitionOnComplete;

    private void OnEnable()
    {
        _fadableComponents = GetComponentsInChildren<IFadable>();
        _parameters.fadeIn.OnTrigger += () =>
        {
            TransitionIn(_parameters.fadeInDurationRange);
        };
        _parameters.fadeOut.OnTrigger += () =>
        {
            TransitionOut(_parameters.fadeOutDurationRange);
        };
    }

    private void OnDisable()
    {
        _parameters.fadeIn.OnTrigger -= () =>
        {
            TransitionIn(_parameters.fadeInDurationRange);
        };
        _parameters.fadeOut.OnTrigger -= () =>
        {
            TransitionOut(_parameters.fadeOutDurationRange);
        };
    }

    public void Initialize()
    {
        // if (_fadableComponents == null)
        // {
        //     _fadableComponents = GetComponentsInChildren<IFadable>();
        // }
        PercentComplete = 0f;
        // set the initial fadables to 0 opacity
        foreach (var fadable in _fadableComponents)
        {
            fadable.SetOpacity(0f);
        }

        State = TransitionState.Idle;
        Debug.Log("Initialized Fade Transition");
    }

    public void TransitionIn(float duration, Action onComplete = null)
    {
        Debug.Log($"CAlling TransitionIn with duration {duration}");
        if (_currentTransitionOnComplete != null)
        {
            CancelTransition();
        }

        State = TransitionState.In;
        float maxDuration = 0f;
        float maxDelay = 0f;
        foreach (var fadable in _fadableComponents)
        {
            var thisDuration =
                UnityEngine.Random.Range(0f, _parameters.fadeInDurationRange) * duration;
            var thisDelay = UnityEngine.Random.Range(0f, _parameters.fadeInDelayRange);
            fadable.FadeIn(thisDuration, thisDelay);
            maxDuration = Mathf.Max(maxDuration, thisDuration);
            maxDelay = Mathf.Max(maxDelay, thisDelay + thisDuration);
        }

        _currentTransitionOnComplete = this.LerpAction(
            (float t) =>
            {
                PercentComplete = t;
            },
            maxDelay + maxDuration + 0.01f,
            () =>
            {
                onComplete?.Invoke();
            }
        );
    }

    public void TransitionOut(float duration, Action onComplete = null)
    {
        if (_currentTransitionOnComplete != null)
        {
            CancelTransition();
        }

        State = TransitionState.Out;
        float maxDuration = 0f;
        float maxDelay = 0f;
        foreach (var fadable in _fadableComponents)
        {
            var thisDuration =
                UnityEngine.Random.Range(0f, _parameters.fadeOutDurationRange) * duration;
            var thisDelay = UnityEngine.Random.Range(0f, _parameters.fadeOutDelayRange);
            fadable.FadeOut(thisDuration, thisDelay);
            maxDuration = Mathf.Max(maxDuration, thisDuration);
            maxDelay = Mathf.Max(maxDelay, thisDelay + thisDuration);
        }
        _currentTransitionOnComplete = this.LerpAction(
            (float t) =>
            {
                PercentComplete = 1 - t;
            },
            maxDelay + maxDuration + 0.01f,
            () =>
            {
                onComplete?.Invoke();
            }
        );
    }

    public void CancelTransition()
    {
        State = TransitionState.Idle;
        if (_currentTransitionOnComplete != null)
        {
            StopCoroutine(_currentTransitionOnComplete);
        }
        foreach (var fadable in _fadableComponents)
        {
            fadable.CancelFade();
        }
    }

    public InsertParameters GetParameters()
    {
        return _parameters;
    }
}
