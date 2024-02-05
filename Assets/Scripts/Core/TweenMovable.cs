using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;


// we might want to set the max tweens capacity to something higher
// Set max Tweeners to 3000 and max Sequences to 200
// DOTween.SetTweensCapacity(3000, 200);

public class TweenMovable : MonoBehaviour
// , IMovable
{
    public TransformSnapshot CurrentSnapshot
    {
        get { return new TransformSnapshot(transform); }
    }

    /** The anchor point which other additional transforms are composited onto **/
    public TransformSnapshot AnchorSnapshot { get; set; }

    /** The target transform to tween to **/
    public TransformSnapshot TargetSnapshot;

    /** The duration of the tween **/
    public float TweenDuration = 0.1f;

    /** The amount of motion applied as composited transforms **/
    public float BlendPercent { get; set; } = 0f;
    private Tweener _currentPositionTween;
    private Tweener _currentRotationTween;
    private Tweener _currentScaleTween;

    private Vector3 _accumulatedPosition = Vector3.zero;
    private Vector3 _accumulatedScale = Vector3.zero;
    private Quaternion _accumulatedRotation = Quaternion.identity;

    public Vector3 TargetPosition => AnchorSnapshot.Position + _accumulatedPosition;
    public Vector3 TargetScale => AnchorSnapshot.Scale + _accumulatedScale;
    public Quaternion TargetRotation => AnchorSnapshot.Rotation * _accumulatedRotation;

    void Start()
    {
        AnchorSnapshot = new TransformSnapshot(transform);
        TargetSnapshot = new TransformSnapshot(transform);
        InitializeTweens();
    }

    /** Updates the anchor transform snapshot to the object's current position **/
    public void UpdateAnchorSnapshot()
    {
        AnchorSnapshot = CurrentSnapshot;
    }

    public void SetTweenDuration(float duration)
    {
        TweenDuration = duration;
        InitializeTweens(duration);
    }

    private void InitializeTweens(float duration = 1.0f)
    {
        _currentPositionTween = transform
            .DOMove(AnchorSnapshot.Position, duration)
            .SetAutoKill(false);
        _currentRotationTween = transform
            .DORotateQuaternion(AnchorSnapshot.Rotation, duration)
            .SetAutoKill(false);
        _currentScaleTween = transform.DOScale(AnchorSnapshot.Scale, duration).SetAutoKill(false);
    }

    public void TransformTo(
        TransformSnapshot snapshot,
        float duration = -1,
        Action onComplete = null
    )
    {
        transform.DOMove(snapshot.Position, duration).OnComplete(() => onComplete?.Invoke());
        transform.DORotateQuaternion(snapshot.Rotation, duration);
        transform.DOScale(snapshot.Scale, duration);
    }

    public void AddPosition(Vector3 position)
    {
        _accumulatedPosition += position;
    }

    public void AddRotation(Quaternion rotation)
    {
        _accumulatedRotation *= rotation;
    }

    public void AddScale(Vector3 scale)
    {
        _accumulatedScale += scale;
    }

    public void SetPosition(Vector3 position, float duration = -1, Action onComplete = null)
    {
        transform.DOMove(position, duration).OnComplete(() => onComplete?.Invoke());
    }

    public void SetRotation(Quaternion rotation, float duration = -1, Action onComplete = null)
    {
        transform.DORotateQuaternion(rotation, duration).OnComplete(() => onComplete?.Invoke());
    }

    public void SetScale(Vector3 scale, float duration = -1, Action onComplete = null)
    {
        transform.DOScale(scale, duration).OnComplete(() => onComplete?.Invoke());
    }

    public void EnvelopeScale(
        float scale,
        float timeAttack,
        float timeRelease,
        Action onComplete = null
    )
    {
        Vector3 newScale = AnchorSnapshot.Scale * scale;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(newScale, timeAttack));
        sequence.Append(transform.DOScale(AnchorSnapshot.Scale, timeRelease));
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    // public void LookAt(Vector3 target, float duration = -1, Action onComplete = null)
    // {
    //     if (duration == -1)
    //         duration = DefaultDuration;
    //     Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
    //     TargetSnapshot.Rotation = targetRotation;
    //     if (onComplete != null)
    //     {
    //         CoroutineHelpers.DelayedAction(onComplete, duration, this);
    //     }
    // }

    public void ResetToDefault(float duration = -1, Action onComplete = null)
    {
        TargetSnapshot = AnchorSnapshot.Copy();
        if (onComplete != null)
        {
            CoroutineHelpers.DelayedAction(onComplete, duration, this);
        }
    }

    public void LerpToDefault(float t)
    {
        AnchorSnapshot = AnchorSnapshot.Copy();
        BlendPercent = t;
    }

    public void LerpToReference(TransformSnapshot reference, float t)
    {
        AnchorSnapshot = reference;
        BlendPercent = t;
    }

    private void ResetAccumulators()
    {
        _accumulatedPosition = Vector3.zero;
        _accumulatedScale = Vector3.zero;
        _accumulatedRotation = Quaternion.identity;
    }

    private void ApplyTargetPosition()
    {
        var target = TargetPosition;
        if (target != transform.position)
        {
            if (BlendPercent > 0f)
                target = Vector3.Lerp(target, AnchorSnapshot.Position, BlendPercent);
            _currentPositionTween.ChangeEndValue(target, true).Restart();
        }
    }

    private void ApplyTargetRotation()
    {
        var target = TargetRotation;
        if (target != transform.rotation)
        {
            if (BlendPercent > 0f)
                target = Quaternion.Lerp(target, AnchorSnapshot.Rotation, BlendPercent);
            _currentRotationTween.ChangeEndValue(target, true).Restart();
        }
    }

    private void ApplyTargetScale()
    {
        var target = TargetScale;
        if (target != transform.localScale)
        {
            if (BlendPercent > 0f)
                target = Vector3.Lerp(target, AnchorSnapshot.Scale, BlendPercent);
            _currentScaleTween.ChangeEndValue(target, true).Restart();
        }
    }

    // make sure this happens after anything changes the position during update
    private void LateUpdate()
    {
        ApplyTargetPosition();
        ApplyTargetRotation();
        ApplyTargetScale();
        ResetAccumulators();
    }
}
