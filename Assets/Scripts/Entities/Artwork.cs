using UnityEngine;
using System.Collections.Generic;


public class Artwork : MonoBehaviour
{
    [SerializeField] private int _index;
    public int Index { get => _index; set => _index = value; }

    public string Id => gameObject.name.Split("Artwork__")[1];

    public List<Motif> Motifs = new List<Motif>();

    public void AddController(IArtworkController controller)
    {
        gameObject.AddComponent(controller.GetType());
        // controller.OnArtworkAdded(this);
    }
    
    private void GetChildMotifs()
    {
        foreach (Transform child in transform)
        {
            // add the motif to the object
            var motif = child.gameObject.AddComponent<Motif>();
            Motifs.Add(motif);
            // var motif = child.GetComponent<Motif>();
            // if (motif != null)
            // {
            //     Motifs.Add(motif);
            // }
        }
    }

    
    public GameObject GetObjectAtNormalizedIndex(float normalizedIndex)
    {
        normalizedIndex *= transform.childCount;
        var index = Mathf.FloorToInt(normalizedIndex);
        var child = transform.GetChild(index);
        return child.gameObject;
    }


    public Motif GetMotifAtNormalizedIndex(float normalizedIndex)
    {
        normalizedIndex *= Motifs.Count;
        var index = Mathf.FloorToInt(normalizedIndex);
        return Motifs[index];
    }




}