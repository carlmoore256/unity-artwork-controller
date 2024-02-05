using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

public abstract class InsertParameters
{
    [JsonIgnore]
    public Dictionary<string, BaseParameterValue> BaseParameters { get; protected set; } =
        new Dictionary<string, BaseParameterValue>();

    public InsertParameters()
    {
        InitializeParameterNames();
    }

    private void InitializeParameterNames()
    {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType.IsSubclassOf(typeof(BaseParameterValue)))
            {
                var paramValue = field.GetValue(this) as BaseParameterValue;
                if (paramValue != null)
                {
                    paramValue.SetName(field.Name);
                    BaseParameters.Add(field.Name, paramValue);
                }
            }
        }
    }

    public void TryPatchParameter(string name, object value)
    {
        if (BaseParameters.TryGetValue(name, out var paramValue))
        {
            paramValue.SetValue(value);
        }
        else
        {
            Debug.LogError($"Could not find parameter {name}");
        }
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

// consider making these things into scriptable objects
[CreateAssetMenu(fileName = "InsertParameters", menuName = "Inserts/InsertParameters", order = 1)]
// TODO - change this from a wrapper, to being an optional class that
// allows us to Patch insert parameters
// this thing just handles patching
public class InsertParametersPatcher : IObservable
{
    private InsertParameters _insertParameters;

    public InsertParametersPatcher(InsertParameters parameters)
    {
        _insertParameters = parameters;
        CacheParameterValues();
    }

    private List<IObserver> _observers = new List<IObserver>();
    protected Dictionary<string, FieldInfo> _parameterFields = new Dictionary<string, FieldInfo>();

    protected void CacheParameterValues()
    {
        var fields = typeof(InsertParameters).GetFields(
            BindingFlags.Public | BindingFlags.Instance
        );
        foreach (var field in fields)
        {
            if (typeof(ParameterValue<>).IsAssignableFrom(field.FieldType))
            {
                _parameterFields.Add(field.Name, field);
            }
        }
    }

    // TODO - call the notifyObservers at some point
    public void RegisterObserver(IObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void UnregisterObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnChanged();
        }
    }

    // public void Patch(BaseParameterValue )

    public void Patch(Dictionary<string, object> data)
    {
        foreach (var pair in data)
        {
            if (_parameterFields.TryGetValue(pair.Key, out var field))
            {
                var paramValue = field.GetValue(_insertParameters);
                var paramType = field.FieldType;
                var setValueMethod = paramType.GetMethod("SetValue");
                if (setValueMethod != null)
                {
                    try
                    {
                        // Convert the value to the correct type before setting
                        var convertedValue = Convert.ChangeType(
                            pair.Value,
                            paramType.GenericTypeArguments[0]
                        );
                        setValueMethod.Invoke(paramValue, new[] { convertedValue });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error setting value for {pair.Key}: {ex.Message}");
                    }
                }
            }
        }
    }
}
