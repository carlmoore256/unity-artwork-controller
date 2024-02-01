using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : MonoBehaviour
{
    public void DeleteAllChildren()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
            children.Add(child.gameObject);

        if (Application.isPlaying)
        {
            children.ForEach(child => Destroy(child));
        }
        else
        {
            children.ForEach(child => DestroyImmediate(child));
        }
    }
}
