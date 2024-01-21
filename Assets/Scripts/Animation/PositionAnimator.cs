using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAnimator : MonoBehaviour, IObjectAnimator<Vector3>
{
    private Interpolator<Vector3> _executor;
    public Interpolator<Vector3> Executor
    {
        get
        {
            if (_executor == null)
            {
                _executor = new Interpolator<Vector3>(
                    this,
                    Vector3.Lerp,
                    AnimationCallback,
                    () => transform.position,
                    curve
                );
            }
            return _executor;
        }
    }

    public Action<Vector3> OnAnimationUpdate;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1));

    private void AnimationCallback(Vector3 newPos)
    {
        transform.position = newPos;
        if (OnAnimationUpdate != null)
        {
            OnAnimationUpdate(newPos);
        }
    }

    public void AnimateTo(Vector3 position, float duration, Action onRequestComplete = null) =>
        Executor.LerpTo(position, duration, onRequestComplete);

    public void AnimateToSnapshot(string key, float duration, Action onRequestComplete = null) =>
        Executor.LerpToSnapshot(key, duration, onRequestComplete);

    public void SetSnapshot(string key) => Executor.SetSnapshot(transform.position, key);

    public void SetSnapshot(Vector3 scale, string key) => Executor.SetSnapshot(scale, key);

    public void SetSnapshot(Vector3 position, string key, float duration)
    {
        Executor.SetSnapshot(position, key);
        Executor.LerpToSnapshot(key, duration);
    }

    public Vector3 GetSnapshot(string key) => Executor.GetSnapshot(key);

    public void Stop() => Executor.Stop();

    public void Resume() => Executor.Resume();

    public void AddUpdateListener(Action<Vector3> listener) => OnAnimationUpdate += listener;

    public void RemoveUpdateListener(Action<Vector3> listener) => OnAnimationUpdate -= listener;
}
