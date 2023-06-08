using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Moveable : MonoBehaviour
{

    public TransformCoroutineManager CoroutineManager;

    public TransformSnapshot CurrentSnapshot { get { return new TransformSnapshot(transform); } }

    public TransformSnapshot DefaultSnapshot;
    private TransformSnapshot _referenceSnapshot;
    public TransformSnapshot TargetSnapshot;

    // private TransformSnapshot _targetSnapshot;

    public float MoveDuration = 0.5f;
    public float DefaultDuration = 0.6f;

    public bool UseLocal = false;

    public UnityAction OnTransformStart; 
    public UnityAction OnTransformEnd;

    public bool EnableLookAtTarget { get; set; }
    public Transform LookAtTarget;

    void Start()
    {
        DefaultSnapshot = new TransformSnapshot(transform);
        TargetSnapshot = new TransformSnapshot(transform);
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

    public void TransformTo(TransformSnapshot snapshot, float duration=-1)
    {
        if (duration == -1) duration = DefaultDuration;
        // CoroutineManager.TransformTo(snapshot, duration);
        TargetSnapshot = snapshot;
    }

    public void MoveTo(Vector3 position, float duration=-1)
    {
        if (duration == -1) duration = DefaultDuration;
        // CoroutineManager.MoveTo(position, duration);
        TargetSnapshot.Position = position;
    }

    public void RotateTo(Quaternion rotation, float duration=-1)
    {
        if (duration == -1) duration = DefaultDuration;
        // CoroutineManager.RotateTo(rotation, duration);
        TargetSnapshot.Rotation = rotation;
    }

    public void ScaleTo(Vector3 scale, float duration=-1)
    {
        if (duration == -1) duration = DefaultDuration;
        // CoroutineManager.ScaleTo(scale, duration);
        TargetSnapshot.Scale = scale;
    }

    public void LookAt(Vector3 target, float duration=-1)
    {
        if (duration == -1) duration = DefaultDuration;
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        // CoroutineManager.RotateTo(targetRotation, duration);
        TargetSnapshot.Rotation = targetRotation;
    }

    public void ResetToDefault(float duration=-1)
    {
        if (duration == -1) duration = DefaultDuration;
        // CoroutineManager.TransformTo(_defaultSnapshot, duration);
        TargetSnapshot = DefaultSnapshot;

    }

    // private Action<TransformSnapshot, TransformSnapshot, float> _lerpFunction;
    private float _lerp = 0f;

    public void LerpToDefault(float t)
    {
        _referenceSnapshot = DefaultSnapshot;
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
            var lerp = TransformSnapshot.Lerp(TargetSnapshot, _referenceSnapshot, _lerp);
            CoroutineManager.TransformTo(lerp, MoveDuration);
        } else {
            CoroutineManager.TransformTo(TargetSnapshot, MoveDuration);
        }

        if (LookAtTarget != null && EnableLookAtTarget)
        {
            // Debug.Log("looking at target " + LookAtTarget.position);
            LookAt(LookAtTarget.position);
        }
    }
}
