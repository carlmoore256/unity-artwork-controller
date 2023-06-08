using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;


public class LayerColor
{
    public Color Color { get; private set; }
    public float Opacity;
    public float FadeOffset;
    public float Rotation;
    public MaskLayer[] Masks;

    private float _curve = 1.5f;

    public LayerColor(MaskLayer[] masks)
    {
        Masks = masks;
        Opacity = 1f;
        Rotation = 0f;
        Color = Color.white;
        _curve = Random.Range(0.5f, 2.5f);
    }

    public LayerColor(MaskLayer mask) : this(new MaskLayer[] { mask }) {}

    public void SetColor(Color color)
    {
        // Color = color;
        Color newColor = new Color(Mathf.Pow(color.r, _curve), Mathf.Pow(color.g, _curve), Mathf.Pow(color.b, _curve), Opacity);
        Color = newColor;


        foreach (var mask in Masks)
        {
            mask.SetColor(newColor);
        }
    }

    public void SetAlpha(float alpha)
    {
        Opacity = Mathf.Pow(alpha, _curve);
        foreach (var mask in Masks)
        {
            mask.SetAlpha(Opacity);
        }
    }
}

// [RequireComponent(typeof(Moveable))]
public class ArtworkColorController : MonoBehaviour, IOscControllable, IArtworkController
{
    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/color";
    private Moveable _moveable;

    // group these if they are in a motif group
    private List<LayerColor> _groupedLayers = new List<LayerColor>();
    private Coroutine _fadeCoroutine;

    void Start()
    {
        _moveable = GetComponent<Moveable>();
        RegisterEndpoints();


        var allMaskLayers = GetComponentsInChildren<MaskLayer>();

        // for each mask layer, if their parent has the tag motifGroup and the parent is the same, 
        // assign them to a group

        Dictionary<string, List<MaskLayer>> motifGroups = new Dictionary<string, List<MaskLayer>>();

        foreach(var layer in allMaskLayers)
        {
            if (layer.transform.parent != null && layer.transform.parent.tag == "motifGroup")
            {
                if (!motifGroups.ContainsKey(layer.transform.parent.name))
                {
                    motifGroups.Add(layer.transform.parent.name, new List<MaskLayer>());
                }
                motifGroups[layer.transform.parent.name].Add(layer);
            } else {
                _groupedLayers.Add(new LayerColor(layer));
            }
        }

        foreach(var group in motifGroups)
        {
            var layerColors = new LayerColor(group.Value.ToArray());
            _groupedLayers.Add(layerColors);
        }


        // initialize to black
        foreach(var layer in _groupedLayers)
        {
            layer.SetColor(Color.white);
            // layer.SetAlpha(0f);
        }
    }

    public void RegisterEndpoints()
    {

        OscManager.Instance.AddEndpoint($"{OscAddress}/opacity", (OscDataHandle dataHandle) => {
            var fade = dataHandle.GetElementAsFloat(0);
            // Artwork.BackgroundFade(fade);
            foreach(var layer in _groupedLayers)
            {
                layer.SetAlpha(fade);
            }
        });
        

        OscManager.Instance.AddEndpoint($"{OscAddress}/rotate", (OscDataHandle dataHandle) => {

            foreach(var layer in _groupedLayers)
            {
                // layer.Rotation = dataHandle.GetElementAsFloat(0);
                Color currentColor = layer.Color;
                Color newColor = new Color(
                    Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f),
                    Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f),
                    Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f),
                    currentColor.a
                );

                layer.SetColor(newColor);
            }

            // var layers = GetComponentsInChildren<MaskLayer>();
            // foreach(var layer in layers)
            // {
            //     Color currentColor = layer.GetColor();
            //     Color newColor = new Color(
            //         Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f),
            //         Mathf.Clamp(dataHandle.GetElementAsFloat(1), 0f, 1f),
            //         Mathf.Clamp(dataHandle.GetElementAsFloat(2), 0f, 1f),
            //         currentColor.a
            //     );

            //     layer.SetColor(newColor);
            // }
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/fadeIn", (OscDataHandle dataHandle) => {
            FadeInEffect();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/fadeOut", (OscDataHandle dataHandle) => {   
            FadeOutEffect();
        });

        // to do 
        // - fade in & out
        // - tweak motion parameter range
        // - get color rotation working

    }

    public void FadeOutEffect(float waitTimeLow = 0f, float waitTimeHigh = 5f, float fadeTimeLow = 0.5f, float fadeTimeHigh = 2f)
    {
        foreach (var layer in _groupedLayers)
        {
            var randomWaitTime = Random.Range(waitTimeLow, waitTimeHigh);
            var randomFadeTime = Random.Range(fadeTimeLow, fadeTimeHigh);
            StartCoroutine(WaitAndFade(layer, randomWaitTime, randomFadeTime, 0f));
        }
    }

    public void FadeInEffect(float waitTimeLow = 0f, float waitTimeHigh = 5f, float fadeTimeLow = 0.5f, float fadeTimeHigh = 2f)
    {
        foreach (var layer in _groupedLayers)
        {
            var randomWaitTime = Random.Range(waitTimeLow, waitTimeHigh);
            var randomFadeTime = Random.Range(fadeTimeLow, fadeTimeHigh);
            StartCoroutine(WaitAndFade(layer, randomWaitTime, randomFadeTime, 1f));
        }
    }

    private IEnumerator WaitAndFade(LayerColor layerColor, float delay, float fadeTime, float fadeTo)
    {
        yield return new WaitForSeconds(delay);
        
        float startTime = Time.time;
        float endTime = startTime + fadeTime;
        float startFade = layerColor.Opacity;
        float endFade = fadeTo;

        while(Time.time < endTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            float fade = Mathf.Lerp(startFade, endFade, t);
            layerColor.SetAlpha(fade);
            yield return null;
        }

    }
}

// OscManager.Instance.AddEndpoint("/controller/moveZ", (OscDataHandle dataHandle) => {
//     Debug.Log("Move Z called! Amount: " + dataHandle.GetElementAsFloat(0));
//     var currentPos = transform.position;
//     currentPos.z = dataHandle.GetElementAsFloat(0);
//     _moveable.MoveTo(currentPos, 0.6f);
// });

// OscManager.Instance.AddEndpoint("/controller/rotateX", (OscDataHandle dataHandle) => {
//     var currentRot = transform.rotation;
//     Vector3 euler = currentRot.eulerAngles;
//     euler.x += dataHandle.GetElementAsFloat(0);
//     _moveable.RotateTo(Quaternion.Euler(euler.x, euler.y, euler.z), 0.6f);
// });

// OscManager.Instance.AddEndpoint("/controller/rotateY", (OscDataHandle dataHandle) => {
//     var currentRot = transform.rotation;
//     Vector3 euler = currentRot.eulerAngles;
//     euler.y += dataHandle.GetElementAsFloat(0);
//     _moveable.RotateTo(Quaternion.Euler(euler.x, euler.y, euler.z), 0.6f);
// });
