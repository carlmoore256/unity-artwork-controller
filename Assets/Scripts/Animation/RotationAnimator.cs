using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimator : MonoBehaviour, IObjectAnimator<Quaternion>
{
    private Interpolator<Quaternion> _executor;
    public Interpolator<Quaternion> Executor
    {
        get
        {
            if (_executor == null)
            {
                _executor = new Interpolator<Quaternion>(
                    this,
                    Quaternion.Slerp,
                    AnimationCallback,
                    () => transform.rotation,
                    curve
                );
            }
            return _executor;
        }
    }

    public Action<Quaternion> OnAnimationUpdate;

    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1));

    private void AnimationCallback(Quaternion newRotation)
    {
        transform.rotation = newRotation;
        if (OnAnimationUpdate != null)
        {
            OnAnimationUpdate(newRotation);
        }
    }

    public void AnimateTo(Quaternion rotation, float duration, Action onRequestComplete = null) =>
        Executor.LerpTo(rotation, duration, onRequestComplete);

    public void AnimateToSnapshot(string key, float duration, Action onRequestComplete = null) =>
        Executor.LerpToSnapshot(key, duration, onRequestComplete);

    public void SetSnapshot(string key) => Executor.SetSnapshot(transform.rotation, key);

    public void SetSnapshot(Quaternion rotation, string key) => Executor.SetSnapshot(rotation, key);

    public void SetSnapshot(Quaternion rotation, string key, float duration)
    {
        Executor.SetSnapshot(rotation, key);
        Executor.LerpToSnapshot(key, duration);
    }

    public Quaternion GetSnapshot(string key) => Executor.GetSnapshot(key);

    public void Stop() => Executor.Stop();

    public void Resume() => Executor.Resume();

    public void AddUpdateListener(Action<Quaternion> listener) => OnAnimationUpdate += listener;

    public void RemoveUpdateListener(Action<Quaternion> listener) => OnAnimationUpdate -= listener;
}
