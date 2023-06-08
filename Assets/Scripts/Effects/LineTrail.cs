using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(LineRenderer))]
public class LineTrail : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private List<Vector3> _positions = new List<Vector3>();

    private int _maxCount = 10;

    private bool _isEnabled = false;
    
    void Awake()
    {
        if (gameObject.GetComponent<LineRenderer>() == null) {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        } else {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        }
    }

    public void Initialize(int count, float startWidth, float endWidth, Material material)
    {
        SetLineCount(count);
        SetLineWidth(startWidth, endWidth);
        SetMaterial(material);
    }

    public void SetLineCount(int count)
    {
        _maxCount = count;
    }

    public void SetLineWidth(float width)
    {
        _lineRenderer.startWidth = width;
        _lineRenderer.endWidth = width;
    }

    public void SetLineWidth(float startWidth, float endWidth)
    {
        _lineRenderer.startWidth = startWidth;
        _lineRenderer.endWidth = endWidth;
    }

    public void SetMaterial(Material material)
    {
        _lineRenderer.material = material;
    }

    void Update()
    {
        if (_positions.Count > 0)
        {
            _lineRenderer.positionCount = _positions.Count;
            _lineRenderer.SetPositions(_positions.ToArray());
        }

        while (_positions.Count > _maxCount)
        {
            _positions.RemoveAt(0);
        }

        _positions.Add(transform.position);
    }

    public void Toggle()
    {
        _isEnabled = !_isEnabled;

        _lineRenderer.enabled = _isEnabled;
    }
}
