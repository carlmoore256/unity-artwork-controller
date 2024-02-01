using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Moveable))]
public class Logo : MonoBehaviour
{

    public string Id => gameObject.name.Split("Logo__")[1];
    private SpriteRenderer _spriteRenderer;
    private CoroutineManager _coroutineManager;

    Moveable _moveable;

    TransformSnapshot _defaultSnapshot;

    public float RotationSpeed = 10.0f;
    public float Scale = 1.0f;

    private float _rotation = 0f;

    private Coroutine _scaleCoroutine;
    private Coroutine _rotationCoroutine;

    void OnEnable()
    {
        _moveable = GetComponent<Moveable>();
        _coroutineManager = new CoroutineManager(this);
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        Debug.Log($"Logo {Id} enabled");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _defaultSnapshot = _moveable.AnchorSnapshot;
        SetOpacity(0f);

      
        _coroutineManager.TimedAction("fade", (value) => {
            SetOpacity(value);
        }, 1f, null, () => {
            Debug.Log("Logo faded in");
        });

        _defaultSnapshot = new TransformSnapshot(transform);
    }

    public void FadeOut()
    {
        _coroutineManager.TimedAction("fade", (value) => {
            SetOpacity(1-value);
        }, 1f, null, () => {
            Debug.Log("Logo faded out");
            gameObject.SetActive(false);
        });
    }

    public void Reset()
    {
        // transform.rotation = Quaternion.identity;
        if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
        if (_rotationCoroutine != null) StopCoroutine(_rotationCoroutine);


        _scaleCoroutine = StartCoroutine(ScaleCoroutine(1f, _defaultSnapshot.Scale));
        _rotationCoroutine = StartCoroutine(RotateCoroutine(1f, _defaultSnapshot.Rotation));
        // transform.localScale = Vector3.one;
    }

    public void ScaleTo(float scale)
    {
        if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
        _scaleCoroutine = StartCoroutine(ScaleCoroutine(1f, _defaultSnapshot.Scale * scale));
    }


    IEnumerator ScaleCoroutine(float duration, Vector3 targetScale)
    {
        float time = 0;
        Vector3 startScale = transform.localScale;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    IEnumerator RotateCoroutine(float duration, Quaternion targetRotation)
    {
        float time = 0;
        Quaternion startRotation = transform.rotation;
        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    void SetOpacity(float value)
    {
        if (_spriteRenderer == null) {
            Debug.LogError("Sprite Renderer is null");
            return;
        };
        Color newColor = _spriteRenderer.color;
        newColor.a = value;
        _spriteRenderer.color = newColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (RotationSpeed > 0) {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);

        }
    }
}
