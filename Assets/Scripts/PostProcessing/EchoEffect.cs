using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode, RequireComponent(typeof(Camera)), RequireComponent(typeof(CameraEffectController))]
public class EchoEffect : MonoBehaviour, ICameraEffect
{
    [SerializeField] private float _fadeDuration = 1f;
    public float FadeDuration => _fadeDuration;
    [SerializeField] private float _intensity = 1f;
    public float Intensity { get => _intensity; set => _intensity = value; }

    public Material effectMaterial;
    public float echoIntensity = 0.5f;
    private RenderTexture prevFrame;

    private Coroutine _fadeCoroutine;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (prevFrame == null || prevFrame.width != source.width || prevFrame.height != source.height)
        {
            if (prevFrame != null)
            {
                DestroyImmediate(prevFrame);
            }

            prevFrame = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            prevFrame.hideFlags = HideFlags.HideAndDontSave;
            Graphics.Blit(source, prevFrame);
        }

        effectMaterial.SetTexture("_PrevTex", prevFrame);
        effectMaterial.SetFloat("_EchoIntensity", echoIntensity);
        effectMaterial.SetFloat("_Intensity", _intensity);

        Graphics.Blit(source, destination, effectMaterial);
        Graphics.Blit(destination, prevFrame);
    }

    private void OnDisable()
    {
        if (prevFrame != null)
        {
            DestroyImmediate(prevFrame);
            prevFrame = null;
        }
    }


    public void FadeIn(Action onComplete = null, float targetIntensity = 1f)
    {
        _intensity = 0;
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(Fade(0, targetIntensity, onComplete));
    }

    public void FadeOut(Action onComplete = null)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(Fade(_intensity, 0, onComplete));
    }

    private IEnumerator Fade(float startIntensity, float endIntensity, Action onComplete = null)
    {
        float startTime = Time.time;
        while (Time.time - startTime < _fadeDuration)
        {
            float t = (Time.time - startTime) / _fadeDuration;
            _intensity = Mathf.Lerp(startIntensity, endIntensity, t);
            yield return null;
        }

        onComplete?.Invoke();
    }
}
