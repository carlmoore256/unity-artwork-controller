using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MaskLayer : MonoBehaviour
{
    private SpriteRenderer _backgroundSprite;
    private List<SpriteRenderer> _imageRenderers = new List<SpriteRenderer>();

    // private float _huePhase = 0;
    private Color _originalColor;

    private void Awake()
    {

        var backgroundSprites = GetComponentsInChildren<SpriteRenderer>();
        backgroundSprites = backgroundSprites.Where(x => x.gameObject.name.Contains("image-orig")).ToArray();
        if (backgroundSprites.Length > 0)
        {
            _backgroundSprite = backgroundSprites[0];
        }

        _originalColor = GetColor();

        // _huePhase = Random.Range(0, 1f);
    }

    public void AddRenderer(SpriteRenderer renderer)
    {
        _imageRenderers.Add(renderer);
    }

    public void AddRenderer(GameObject objectRenderer)
    {
        if (objectRenderer.GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogError("Object does not have a sprite renderer!");
            return;
        }
        objectRenderer.transform.SetParent(transform);
        AddRenderer(objectRenderer.GetComponent<SpriteRenderer>());
    }

    public void SetColor(Color color)
    {
        // _backgroundSprite.material.SetColor("_Color", color);
        _backgroundSprite.material.SetColor("_Color", color);
    }

    public void SetHueOffset(float offset)
    {
        // Color currentColor = GetColor();

        // get the hsl of the current
        float H, S, V;
        Color.RGBToHSV(_originalColor, out H, out S, out V);

        // Debug.Log("Hue: " + H + " Offset: " + offset + " New Hue: " + (H + offset));

        // set saturation to a cosine
        S = Mathf.Cos((offset * Mathf.PI * 2f) + Mathf.PI) * 0.5f + 0.5f;

        // add the offset
        H += offset;
        H = H % 1f;

        // set the new color
        Color newColor = Color.HSVToRGB(H, S, V);
        // Debug.Log("old color" + _originalColor + "New Color: " + newColor);
        SetColor(newColor);
    }

    // public void SetSaturationOffset()

    public void ResetColor()
    {
        SetColor(_originalColor);
    }


    public void SetAlpha(float alpha)
    {
        Color newColor = _backgroundSprite.material.GetColor("_Color");
        newColor.a = alpha;
        _backgroundSprite.material.SetColor("_Color", newColor);
    }

    public Color GetColor()
    {
        if (_backgroundSprite != null)
        {
            return _backgroundSprite.material.GetColor("_Color");
        }
        else
        {
            return Color.white;
        }
    }

}
