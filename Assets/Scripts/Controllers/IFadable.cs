public interface IFadable
{
    void SetOpacity(float opacity);
    float GetOpacity();
    void FadeIn(float duration, float delay = 0f);
    void FadeOut(float duration, float delay = 0f);
    void CancelFade();
}