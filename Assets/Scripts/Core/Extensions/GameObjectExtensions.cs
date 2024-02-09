using UnityEngine;
using System;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject gameObject)
        where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    public static string GetUniqueComponentId(this Component component)
    {
        // Get all components of the same type on the GameObject
        var componentType = component.GetType();
        var components = component.gameObject.GetComponents(componentType);

        // Find the index of the current component in the array
        int index = Array.IndexOf(components, component);
        if (index == -1)
        {
            // This should not happen, but included for safety
            throw new InvalidOperationException("Component not found on its own GameObject.");
        }

        // If there are multiple components of the same type, include the index in the ID
        return $"{component.gameObject.name}_{componentType.Name}_{index}";
    }
}

public class MonoBehaviourWithId : MonoBehaviour
{
    public string Id
    {
        get
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = this.GetUniqueComponentId();
            }
            return _id;
        }
    }

    private string _id;
}
