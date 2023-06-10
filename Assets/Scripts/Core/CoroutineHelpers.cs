using UnityEngine;
using System;
using System.Collections;

public static class CoroutineHelpers
{
    private static IEnumerator DelayedAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public static void DelayedAction(Action action, float delay, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(DelayedAction(action, delay));
    }

    public static void LerpAction(Action<float> action, Action onComplete, float duration, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(LerpAction(action, duration));
    }


    private static IEnumerator LerpAction(Action<float> action, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            action(t);
            yield return null;
        }
    }

    private static IEnumerator ChangeColor(object obj, Color startColor, Color endColor, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            Color newColor = Color.Lerp(startColor, endColor, t);
            if (obj == null)
            {
                yield break;
            }
            obj.GetType().GetProperty("color").SetValue(obj, newColor, null);
            yield return null;
        }
    }

    public static void ChangeColor(object obj, Color startColor, Color endColor, float duration, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(ChangeColor(obj, startColor, endColor, duration));
    }
}