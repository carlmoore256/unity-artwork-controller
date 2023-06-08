using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MaskLayer : MonoBehaviour
{
    private SpriteRenderer _backgroundSprite;
    private float _phase = 0;
    private List<SpriteRenderer> _imageRenderers = new List<SpriteRenderer>();

    private void Awake()
    {

        var backgroundSprites = GetComponentsInChildren<SpriteRenderer>();
        backgroundSprites = backgroundSprites.Where(x => x.gameObject.name.Contains("image-orig")).ToArray();
        if (backgroundSprites.Length > 0)
        {
            _backgroundSprite = backgroundSprites[0];
        }

        _phase = Random.Range(0, 1f);
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
        _backgroundSprite.material.SetColor("_Color", color);
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
