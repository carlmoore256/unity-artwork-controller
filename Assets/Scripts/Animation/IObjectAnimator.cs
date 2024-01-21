using System;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectAnimator<T>
{
    public Interpolator<T> Executor { get; }

    void AnimateTo(T target, float duration, Action onComplete = null);
    void AnimateToSnapshot(string key, float duration, Action onComplete = null);
    void SetSnapshot(T target, string key);
    void SetSnapshot(T target, string key, float duration);

    T GetSnapshot(string key);

    void AddUpdateListener(Action<T> listener);
    void RemoveUpdateListener(Action<T> listener);
    void Stop();
    void Resume();
}
