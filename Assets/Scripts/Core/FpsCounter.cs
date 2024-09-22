using UnityEngine;
using System;

public class FpsCounter : MonoBehaviour
{
    public float fpsRefreshTime = 0.5f;
    public Action<float> OnFpsChanged;
    public float FrameRate => _lastFramerate;

    private int _frameCounter = 0;
    private float _timeCounter = 0.0f;
    private float _lastFramerate = 0.0f;

    private void Update()
    {
        if (_timeCounter < fpsRefreshTime)
        {
            _timeCounter += Time.deltaTime;
            _frameCounter++;
        }
        else
        {
            _lastFramerate = (float)_frameCounter / _timeCounter;
            _frameCounter = 0;
            _timeCounter = 0.0f;
            OnFpsChanged?.Invoke(_lastFramerate);
        }
    }
}
