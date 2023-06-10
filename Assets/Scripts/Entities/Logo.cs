using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Moveable))]
public class Logo : MonoBehaviour
{

    public string Id => gameObject.name.Split("Logo__")[1];
    private SpriteRenderer _spriteRenderer;
    private CoroutineManager _coroutineManager;

    // Moveable _moveable;

    TransformSnapshot _defaultSnapshot;

    void OnEnable()
    {
        // _moveable = GetComponent<Moveable>();
        _coroutineManager = new CoroutineManager(this);
        gameObject.name = gameObject.name.Replace("(Clone)", "");
        Debug.Log($"Logo {Id} enabled");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // _defaultSnapshot = _moveable.CurrentSnapshot;
        SetOpacity(0f);

      
        _coroutineManager.TimedAction("fade", (value) => {
            SetOpacity(value);
        }, 1f, null, () => {
            Debug.Log("Logo faded in");
        });
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

    // Start is called before the first frame update
    void Start()
    {
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
        // var rot = transform.rotation.eulerAngles.z + 0.1f * 10f * Time.deltaTime;
        // _moveable.RotateTo(Quaternion.Euler(0, 0, rot));

        // _moveable.ScaleTo(_defaultSnapshot.Scale * 1.1f);
        // _defaultSnapshot.Scale *= 1.1f;

        // ROTATE AROUND 
        transform.rotation = Quaternion.Euler(0,  transform.rotation.eulerAngles.z + 0.1f * 10f * Time.deltaTime,  0);
    }
}
