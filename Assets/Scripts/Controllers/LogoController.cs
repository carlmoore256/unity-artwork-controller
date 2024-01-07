using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class LogoController : MonoBehaviour, INetworkEndpoint
{
    public string Address => "/logo";

    public Logo[] Logos => GetComponentsInChildren<Logo>(true);

    private GameObject[] _logoPrefabs;

    private Logo _activeLogo;

 
    void Start()
    {
        // _logos = GetComponentsInChildren<Logo>();
        // foreach(var logo in _logos)
        // {
        //     Debug.Log(logo.Id);
        //     // logo.gameObject.SetActive(false);
        // }

    }

    void OnEnable()
    {
        _logoPrefabs = Resources.LoadAll<GameObject>("Logos");
        if (_logoPrefabs.Length == 0) {
            throw new System.Exception($"No Logo Prefabs found in Resources/Logos");
        }

        foreach(var logoPrefab in _logoPrefabs)
        {
            CreateNewLogo(logoPrefab);
        }


        RegisterEndpoints();
    }


    void OnDisable()
    {
        UnregisterEndpoints();
    }

    void OnDestroy()
    {
        UnregisterEndpoints();
    }

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{Address}/enableLogo", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Enable Logo {value}");
            EnableLogo(value);
        });

        OscManager.Instance.AddEndpoint($"{Address}/disableLogo", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Disable Logo {value}");
            DisableLogo(value);
        });

        OscManager.Instance.AddEndpoint($"{Address}/toggleLogo", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Toggle Logo {value}");
            ToggleLogo(value);
        });

        OscManager.Instance.AddEndpoint($"{Address}/rotateSpeed", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsFloat(0);
            Debug.Log($"Rotate Speed {value}");
            if (_activeLogo != null) {
                _activeLogo.RotationSpeed = value;
            }
        });

        OscManager.Instance.AddEndpoint($"{Address}/scale", (OscDataHandle dataHandle) => {
            var value = Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0.1f, 3f);
            Debug.Log($"Scale {value}");
            if (_activeLogo != null) {
                // _activeLogo.Scale = value;
                _activeLogo.ScaleTo(value);
            }
        });

        OscManager.Instance.AddEndpoint($"{Address}/reset", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsFloat(0);
            Debug.Log($"Reset {value}");
            if (_activeLogo != null) {
                _activeLogo.Reset();
            }
        });
    }

    public void UnregisterEndpoints()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveEndpoint($"{Address}/enableLogo");
        OscManager.Instance.RemoveEndpoint($"{Address}/disableLogo");
        OscManager.Instance.RemoveEndpoint($"{Address}/toggleLogo");
    }

    private void EnableLogo(string id)
    {
        var logo = Logos.FirstOrDefault(l => l.Id.ToLower() == id.ToLower());
        logo.gameObject.SetActive(true);
    }

    private void DisableLogo(string id)
    {
        var logo = Logos.FirstOrDefault(l => l.Id.ToLower() == id.ToLower());
        logo.FadeOut();
    }

    private GameObject FindLogoObject(string id)
    {
        GameObject logo = null;
        foreach(var l in Logos)
        {
            var nameParts = l.name.Split("Logo__");
            if (nameParts.Length < 2) continue;
            var name = nameParts[1].ToLower();
            if (name == id.ToLower())
            {
                logo = l.gameObject;
                break;
            }
        }
        return logo;
    }

    private void CreateNewLogo(GameObject logoPrefab)
    {
        GameObject newLogo = Instantiate(logoPrefab, transform);
        var currentName = newLogo.name;
        newLogo.SetActive(false);
    }

    private void ToggleLogo(string id)
    {
        Debug.Log("ALl logos" + Logos.Length);
        var logo = Logos.FirstOrDefault(l => l.Id.ToLower() == id.ToLower());
        if (logo == null)
        {
            return;
        }
        if (logo.gameObject.activeSelf)
        {
            logo.FadeOut();
            _activeLogo = null;
        }
        else
        {
            logo.gameObject.SetActive(true);
            _activeLogo = logo;
        }
    }
}
