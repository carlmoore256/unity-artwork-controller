using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode, RequireComponent(typeof(Camera)), RequireComponent(typeof(PostProcessingController))]
public class KaleidoscopeEffect : MonoBehaviour, ICameraEffect
{
    [SerializeField] private float _fadeDuration = 1f;
    public float FadeDuration => _fadeDuration;

    [SerializeField] private float _intensity = 1f;
    public float Intensity { get => _intensity; set => _intensity = value; }
    
    public Material effectMaterial;

    [Range(1, 20)]
    public int reflections = 4;
    public float stretch = 1f;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(effectMaterial != null)
        {
            effectMaterial.SetInt("_Reflections", reflections);
            effectMaterial.SetFloat("_Stretch", stretch);
            effectMaterial.SetFloat("_Intensity", _intensity);
            
            Graphics.Blit(source, destination, effectMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    
    public void FadeIn(Action onComplete = null, float targetIntensity = 1f)
    {
        _intensity = 0;
        StartCoroutine(Fade(0, targetIntensity, onComplete));
    }

    public void FadeOut(Action onComplete = null)
    {
        StartCoroutine(Fade(1, 0, onComplete));
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
