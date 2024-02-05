using System.Collections.Generic;
using System.Collections.Generic;
using System.Reflection;

public interface IDictionaryConvertable
{
    Dictionary<string, object> ToDictionary();
}

public static class DictionaryExtensions
{
    public static Dictionary<string, object> ConvertToDictionary(this object obj)
    {
        var dictionary = new Dictionary<string, object>();

        // For fields:
        foreach (
            var field in obj.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        )
        {
            var value = field.GetValue(obj);
            if (value is IDictionaryConvertable)
            {
                dictionary[field.Name] = ((IDictionaryConvertable)value).ToDictionary();
            }
            else
            {
                dictionary[field.Name] = value;
            }
        }

        // For properties:
        foreach (
            var prop in obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        )
        {
            var value = prop.GetValue(obj, null);
            if (value is IDictionaryConvertable)
            {
                dictionary[prop.Name] = ((IDictionaryConvertable)value).ToDictionary();
            }
            else
            {
                dictionary[prop.Name] = value;
            }
        }

        return dictionary;
    }
}
