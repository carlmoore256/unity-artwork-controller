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

        // _targetPosition = snapshot.Position;
        // _targetRotation = snapshot.Rotation;
        // _targetScale = snapshot.Scale;

        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void MoveTo(Vector3 position, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot.Position = position;

        // _targetPosition = position;

        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void RotateTo(Quaternion rotation, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot.Rotation = rotation;

        // _targetRotation = rotation;

        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void ScaleTo(Vector3 scale, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot.Scale = scale;

        // _targetScale = scale;

        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void LookAt(Vector3 target, float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        TargetSnapshot.Rotation = targetRotation;

        // _targetRotation = targetRotation;

        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void ResetToDefault(float duration=-1, Action onComplete=null)
    {
        if (duration == -1) duration = DefaultDuration;
        TargetSnapshot = DefaultSnapshot.Copy();

        // var defaultSnapshot = DefaultSnapshot.Copy();
        // _targetPosition = defaultSnapshot.Position;
        // _targetRotation = defaultSnapshot.Rotation;
        // _targetScale = defaultSnapshot.Scale;


        if (onComplete != null) {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }


    public void LerpToDefault(float t)
    {
        _referenceSnapshot = DefaultSnapshot.Copy();
        _lerp = t;
        // _lerpFunction = (_defaultSnapshot, _targetTransform.ToSnapshot(), t) => {
        //     var test = TransformSnapshot.Lerp(_defaultSnapshot, _targetTransform.ToSnapshot(), t);
        // };
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
            // var posLerp = Vector3.Lerp(_referenceSnapshot.Position, _targetPosition, _lerp);
            // var rotLerp = Quaternion.Lerp(_referenceSnapshot.Rotation, _targetRotation, _lerp);
            // var scaleLerp = Vector3.Lerp(_referenceSnapshot.Scale, _targetScale, _lerp);
            // CoroutineManager.MoveTo(posLerp, MoveDuration);
            // CoroutineManager.RotateTo(rotLerp, MoveDuration);
            // CoroutineManager.ScaleTo(scaleLerp, MoveDuration);

        } else {
            CoroutineManager.TransformTo(TargetSnapshot, MoveDuration);
            // CoroutineManager.MoveTo(_targetPosition, MoveDuration);
            // CoroutineManager.RotateTo(_targetRotation, MoveDuration);
            // CoroutineManager.ScaleTo(_targetScale, MoveDuration);
        }
    }

}
