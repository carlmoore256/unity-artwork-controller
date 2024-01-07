using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;


// CONSIDER REMOVING! ARTWORK TAKES CARE OF THIS
public class MotifController : MonoBehaviour, INetworkEndpoint, IArtworkController
{
    public Artwork Artwork => GetComponent<Artwork>();
    public string Address => $"artwork/{Artwork.Id}/motif";

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

    void OnEnable()
    {
        RegisterEndpoints();
    }

    void OnDisable()
    {
        OscManager.Instance.RemoveEndpoint($"{Address}/bgFade");
    }

    void OnDestroy()
    {
        UnregisterEndpoints();
    }


    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{Address}/bgFade", (OscDataHandle dataHandle) => {
            var fade = dataHandle.GetElementAsFloat(0);
            // Artwork.BackgroundFade(fade);
        });
    }

    public void UnregisterEndpoints()
    {
        OscManager.Instance.RemoveEndpoint($"{Address}/bgFade");
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
