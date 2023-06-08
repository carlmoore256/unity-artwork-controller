using System;

public interface ICameraEffect
{
    float FadeDuration { get; }
    float Intensity { get; set; }
    void FadeIn(Action onComplete, float targetIntensity);
    void FadeOut(Action onComplete);
}
