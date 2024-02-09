using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;

public class BaseParameterValue
{
    public string name;
    public string description;

    // public virtual object Value { get; prote}

    public virtual void SetName(string name)
    {
        this.name = name;
    }

    public virtual string Type => this.GetType().Name;

    public virtual void SetValue(object val)
    {
        Debug.Log("BaseParameterValue.SetValue called");
    }
}

[System.Serializable]
public class ParameterValue<T> : BaseParameterValue
{
    public T value;
    public T defaultValue;

    public event Action<T> OnValueChanged;

    public ParameterValue(T defaultValue)
    {
        value = defaultValue;
        this.defaultValue = defaultValue;
    }

    public ParameterValue(T defaultValue, string description)
        : this(defaultValue)
    {
        this.description = description;
    }

    public override void SetValue(object val)
    {
        Debug.Log($"ParameterValue.SetValue called with {val}");
        if (val is T t)
        {
            Debug.Log($"ParameterValue.SetValue called with {t} | {val}");
            SetValue(t);
        }
    }

    public virtual void SetValue(T val)
    {
        if (!EqualityComparer<T>.Default.Equals(value, val))
        {
            value = val;
            OnValueChanged?.Invoke(val);
        }
    }

    public override void SetName(string name)
    {
        this.name = name;
    }

    // Implicit conversion from ParameterValue<T> to T
    public static implicit operator T(ParameterValue<T> parameterValue)
    {
        return parameterValue.value;
    }

    public void Reset()
    {
        value = defaultValue;
    }

    // return the generic type of the parameter
    public override string Type => typeof(T).Name;

    // public override string Type => this.GetType().Name;

    // Implicit conversion from T to ParameterValue<T>
    // public static implicit operator ParameterValue<T>(T value)
    // {
    //     return new ParameterValue<T> { value = value };
    // }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

[System.Serializable]
public class RangedParameterValue : ParameterValue<float>
{
    public float min;
    public float max;

    public RangedParameterValue(float defaultValue, float min, float max)
        : base(defaultValue)
    {
        this.min = min;
        this.max = max;
    }

    public RangedParameterValue(float defaultValue, float min, float max, string description)
        : base(defaultValue, description)
    {
        this.min = min;
        this.max = max;
    }

    // create a new RangedParameterValue with a default value of 0, and a range of 0-1
    public static RangedParameterValue NormalizedRange(string description, float defaultValue = 0f)
    {
        if (defaultValue < 0f || defaultValue > 1f)
            throw new ArgumentException("Default value must be between 0 and 1");
        return new RangedParameterValue(defaultValue, 0f, 1f, description);
    }

    public bool IsWithinRange(float val)
    {
        return val >= min && val <= max;
    }

    // public override void SetValue(object val)
    // {
    //     if (val is float v)
    //     {
    //         Debug.Log($"RangedParameterValue.SetValue called with {v}");
    //         SetValue(v as float?);
    //     }
    // }

    public override void SetValue(object val)
    {
        // if its a double, convert it to a float
        if (val is double d)
        {
            SetValue((float)d);
        }
        else
        {
            SetValue((float)val);
        }
    }

    public override void SetValue(float val)
    {
        if (IsWithinRange(val))
        {
            base.SetValue(val);
        }
        else
        {
            Debug.LogWarning($"Value {val} is not within range {min} - {max}");
        }
    }

    public static implicit operator float(RangedParameterValue parameterValue)
    {
        return parameterValue.value;
    }

    public override string Type => "RangedFloat";
}

public class TriggerParameterValue : BaseParameterValue
{
    public Action OnTrigger;
    
    public TriggerParameterValue(string description = "Trigger")
    {
        this.description = description;
    }

    public override void SetValue(object val)
    {
        OnTrigger?.Invoke();
    }

    public override string Type => "Trigger";
}
