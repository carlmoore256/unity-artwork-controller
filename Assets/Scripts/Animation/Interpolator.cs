using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;


public class Interpolator<T>
{
    public AnimationCurve Curve;
    public Dictionary<string, T> Snapshots
    {
        get
        {
            // make sure we only construct if we're going to use it
            if (_snapshots == null) _snapshots = new Dictionary<string, T>();
            return _snapshots;
        }
    }

    // public Func<T, float, T> Callback; // input T, 0-1 float, output T
    // public Func<T, T, float, T> LerpFunction; // start T, end T, 0-1 float, output T
    public Func<T, T, float, T> LerpFunction;
    public Action<T> OnUpdate;
    public Action<T> OnComplete;
    public Func<T> GetCurrent;

    private T _startValue;
    private T _endValue;
    private T _currentValue;

    private float _currentT;
    private float _startTime;
    private float _endTime;

    private bool _isStopped;

    private Dictionary<string, T> _snapshots;

    private Coroutine _coroutine;
    private MonoBehaviour _monoBehaviour;

    private Action _onRequestComplete; // single use onComplete that occurs when a new request is made | WARNING - might perform in unexpected ways

    public Interpolator(
        MonoBehaviour monoBehaviour,
        Func<T, T, float, T> lerpFunction,
        Action<T> onUpdate,
        Func<T> getCurrent,
        AnimationCurve curve,
        Action<T> onComplete = null)
    {
        _monoBehaviour = monoBehaviour;
        LerpFunction = lerpFunction;
        OnUpdate = onUpdate;
        GetCurrent = getCurrent;
        Curve = curve;
        OnComplete = onComplete;
    }

    public void SetSnapshot(T value, string key)
    {
        if (Snapshots.ContainsKey(key))
        {
            Snapshots[key] = value;
            return;
        }
        Snapshots.Add(key, value);
    }

    public T GetSnapshot(string key)
    {
        if (Snapshots.ContainsKey(key))
        {
            return Snapshots[key];
        }
        Debug.LogError($"[Lerp Executor] Snapshot {key} doesn't exist!");
        return default(T);
    }

    public void LerpToSnapshot(string key, float duration, Action onRequestComplete = null)
    {
        if (_snapshots.ContainsKey(key))
        {
            LerpTo(_snapshots[key], duration, onRequestComplete);
            return;
        }
        Debug.LogError($"[Lerp Executor] Snapshot {key} doesn't exist!");
    }

    public void LerpTo(T value, float duration, Action onRequestComplete = null)
    {
        _currentValue = GetCurrent();

        _startTime = Time.time;
        _endTime = _startTime + duration;
        _startValue = _currentValue;
        _endValue = value;
        if (onRequestComplete != null)
            _onRequestComplete = onRequestComplete; // this admittedly will act very strangely if you're not expecting it, since
            // coroutines get delayed and continue to call if double called, rather than re-instantiated. Not sure how to resolve this
            // madness other than just accept it
        if (_coroutine == null) // if it's expired, start it 
        {
            _coroutine = _monoBehaviour.StartCoroutine(Lerp());
        }
    }

    public void Stop()
    {
        if (_coroutine != null) _monoBehaviour.StopCoroutine(_coroutine);
        _coroutine = null;
        _isStopped = true;
    }

    public void Resume()
    {
        if (!_isStopped)
        {
            throw new Exception("Can't resume a coroutine that hasn't been stopped!");
        }
        float duration = _endTime - _startTime;
        _startTime = Time.time;
        _endTime = duration * (1f - _currentT) + Time.time;
        _coroutine = _monoBehaviour.StartCoroutine(Lerp());
    }

    private IEnumerator Lerp()
    {
        while (Time.time < _endTime)
        {
            float t = (Time.time - _startTime) / (_endTime - _startTime);
            // if (Curve > 1f || Curve < 1f) t = Mathf.Pow(t, Curve);
            float curveValue = Curve.Evaluate(t);
            _currentValue = LerpFunction(_startValue, _endValue, curveValue);
            OnUpdate?.Invoke(_currentValue);
            _currentT = t;
            yield return null;
        }
        // Callback(_endValue, 1f);
        OnComplete?.Invoke(_endValue);
        _onRequestComplete?.Invoke();
        _onRequestComplete = null;
        _coroutine = null;
        yield return null;
    }
}
