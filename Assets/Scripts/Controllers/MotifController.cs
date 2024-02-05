using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;


// CONSIDER REMOVING! ARTWORK TAKES CARE OF THIS
public class MotifController : MonoBehaviour, INetworkEndpoint, IArtworkController
{
    public SegmentedPaintingArtwork Artwork => GetComponent<SegmentedPaintingArtwork>();
    public string Address => $"/motif";

    [SerializeField] private GameObject _background;

    void Start() 
    {
        if (_background == null)
        {
            var bg = gameObject.transform.Find("image-orig(Clone)");
            if (bg != null) {
                _background = bg.gameObject;
            }
        }
    }

    // void OnEnable()
    // {
    //     Register();
    // }

    void OnDisable()
    {
        // OscManager.Instance.RemoveEndpoint($"{Address}/bgFade");
    }

    void OnDestroy()
    {
        Unregister();
    }


    public void Register(string baseAddress)
    {
        OscManager.Instance.AddEndpoint($"{baseAddress}/motif/bgFade", (OscDataHandle dataHandle) => {
            var fade = dataHandle.GetElementAsFloat(0);
            // Artwork.BackgroundFade(fade);
        }, this);
    }

    public void Unregister()
    {
        // OscManager.Instance.RemoveEndpoint($"{Address}/bgFade");
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
    }


    GameObject GetChildWithName(GameObject obj, string name) {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null) {
            return childTrans.gameObject;
        } else {
            return null;
        }
    }
}
