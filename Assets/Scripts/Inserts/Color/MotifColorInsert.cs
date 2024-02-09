using System;
using UnityEngine;

public class MotifColorInsertParameters : InsertParameters
{
    // public ParameterValue<Color> tint = new ParameterValue<Color>(Color.white, "Motif tint");
    public RangedParameterValue opacity = RangedParameterValue.NormalizedRange("Opacity");
    public RangedParameterValue hueOffset = RangedParameterValue.NormalizedRange("Hue Offset");
    public RangedParameterValue hueOffsetScale = RangedParameterValue.NormalizedRange(
        "Hue Offset Scale"
    );
}

public class MotifColorInsert : MonoBehaviourWithId, IInsert
{
    public string Name => "Motif Color";

    // this should really be a color iterator!
    // OR we could make the type of iterator more generic somehow...
    private IMotifIterator _motifIterator;
    private MotifColorInsertParameters _parameters = new MotifColorInsertParameters();

    public InsertParameters GetParameters()
    {
        return _parameters;
    }

    private float _hueOffsetScale = 0f;
    private float _currentHueOffset = 0f;

    private void OnEnable()
    {
        _motifIterator = GetComponent<IMotifIterator>();
        _parameters.opacity.OnValueChanged += OnOpacityChanged;
        _parameters.hueOffset.OnValueChanged += OnHueOffsetChanged;
        _parameters.hueOffsetScale.OnValueChanged += OnHueOffsetScaleChanged;
    }

    private void OnDisable()
    {
        _parameters.opacity.OnValueChanged -= OnOpacityChanged;
        _parameters.hueOffset.OnValueChanged -= OnHueOffsetChanged;
        _parameters.hueOffsetScale.OnValueChanged -= OnHueOffsetScaleChanged;
    }

    private void OnOpacityChanged(float newOpacity)
    {
        _motifIterator.ForeachMotif(
            (motif) =>
            {
                motif.SetOpacity(newOpacity);
            }
        );
    }

    private void OnHueOffsetChanged(float newHueOffset)
    {
        _currentHueOffset = newHueOffset;
        _motifIterator.ForeachMotif(
            (motif, index) =>
            {
                motif.SetHueOffset(_currentHueOffset + (index * _hueOffsetScale));
            }
        );

        // add lerp to this
    }

    private void OnHueOffsetScaleChanged(float newHueOffsetScale)
    {
        _hueOffsetScale = ((Mathf.Cos(newHueOffsetScale * Mathf.PI * 2f) * 0.5f + 0.5f) * -1) + 1f;
        OnHueOffsetChanged(_currentHueOffset);
    }
}
