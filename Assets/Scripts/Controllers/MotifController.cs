using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;

public class MotifController : MonoBehaviour, IOscControllable, IArtworkController
{

    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"artwork/{Artwork.Id}/motif";

    [SerializeField] private GameObject _background;

    void Start() 
    {
        RegisterEndpoints();

        if (_background == null)
        {
            var bg = gameObject.transform.Find("image-orig(Clone)");
            if (bg != null) {
                _background = bg.gameObject;
            }
        }
    }

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{OscAddress}/bgFade", (OscDataHandle dataHandle) => {
            var fade = dataHandle.GetElementAsFloat(0);
            // Artwork.BackgroundFade(fade);
        });
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
