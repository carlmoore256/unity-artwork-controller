using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Moveable : MonoBehaviour
{

    public TransformCoroutineManager CoroutineManager;

    public TransformSnapshot CurrentSnapshot { get { return new TransformSnapshot(transform); } }


    public Vector3 DefaultPosition;
    public Quaternion DefaultRotation;
    public Vector3 DefaultScale;

    public TransformSnapshot DefaultSnapshot;
    private TransformSnapshot _referenceSnapshot;
    public TransformSnapshot TargetSnapshot;

    public float MoveDuration = 0.5f;
    public float DefaultDuration = 0.6f;

    public bool UseLocal = false;

    public UnityAction OnTransformStart; 
    public UnityAction OnTransformEnd;
    private float _lerp = 0f;

    // private Vector3 _targetPosition;
    // private Vector3 _targetScale;
    // private Quaternion _targetRotation;
    // public TransformSnapshot TargetSnapshot => new TransformSnapshot(_targetPosition, _targetRotation, _targetScale);

    void Start()
    {
        DefaultSnapshot = new TransformSnapshot(transform);
        TargetSnapshot = new TransformSnapshot(transform);

        DefaultPosition = transform.position;
        DefaultRotation = transform.rotation;
        DefaultScale = transform.localScale;
    }

    void OnEnable()
    {
        CoroutineManager = new TransformCoroutineManager(this, 
        ()=>{
            OnTransformStart?.Invoke();
        }, 
        ()=> {
            OnTransformEnd?.Invoke();
        }, 
        UseLocal);
    }

    public void TransformTo(TransformSnapshot snapshot, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot = snapshot;
        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void MoveTo(Vector3 position, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot.Position = position;
        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void RotateTo(Quaternion rotation, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot.Rotation = rotation;
        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void ScaleTo(Vector3 scale, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot.Scale = scale;
        
        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void EnvelopeScale(float scale, float timeAttack, float timeRelease, Action onComplete=null)
    {
        Vector3 newScale = DefaultSnapshot.Scale * scale;
        if (_envelopeScaleCoroutine != null) StopCoroutine(_envelopeScaleCoroutine);
        _envelopeScaleCoroutine = StartCoroutine(EnvelopeScaleCoroutine(newScale, DefaultSnapshot.Scale, timeAttack, timeRelease, onComplete));
    }

    public void LookAt(Vector3 target, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        TargetSnapshot.Rotation = targetRotation;
        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void ResetToDefault(float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot = DefaultSnapshot.Copy();
        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }


    public void LerpToDefault(float t)
    {
        _referenceSnapshot = DefaultSnapshot.Copy();
        _lerp = t;
    }

    public void LerpToReference(TransformSnapshot reference, float t)
    {
        _referenceSnapshot = reference;
        _lerp = t;
    }

    private void Update()
    {
        // now we have to lerp between _referenceSnapshot and _targetSnapshot
        if (_lerp > 0f) 
        {
            var lerp = TransformSnapshot.NewFromLerp(TargetSnapshot, _referenceSnapshot, _lerp);
            CoroutineManager.TransformTo(lerp, MoveDuration);
        } else {
            CoroutineManager.TransformTo(TargetSnapshot, MoveDuration);
        }
    }

    private Coroutine _envelopeScaleCoroutine;

    private IEnumerator EnvelopeScaleCoroutine(Vector3 newScale, Vector3 endScale, float timeAttack, float timeRelease, Action onComplete=null)
    {
        var t = 0f;
        Vector3 initialScale = transform.localScale;
        while (t < 1)
        {
            t += Time.deltaTime / timeAttack;
            transform.localScale = Vector3.Lerp(initialScale, newScale, t);
            yield return null;
        }
        transform.localScale = newScale;
        t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeRelease;
            transform.localScale = Vector3.Lerp(newScale, endScale, t);
            yield return null;
        }
        transform.localScale = endScale;
        if (onComplete != null) onComplete();
    }


    private Coroutine _scaleToCoroutine;
    private IEnumerator ScaleToCoroutine(Vector3 newScale, Vector3 originalScale, float durationAttack, float durationDecay)
    {
        var t = 0f;
        Vector3 initialScale = transform.localScale;
        while (t < durationAttack)
        {
            t += Time.deltaTime / durationAttack;
            transform.localScale = Vector3.Lerp(initialScale, newScale, t);
            yield return null;
        }
        transform.localScale = newScale;
        t = 0f;
        while (t < durationDecay)
        {
            t += Time.deltaTime / durationDecay;
            transform.localScale = Vector3.Lerp(newScale, originalScale, t);
            yield return null;
        }
        transform.localScale = originalScale;
    }


}
