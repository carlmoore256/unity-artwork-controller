using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class LogoController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/logo";

    private Logo[] _logos;

 
    void Start()
    {
        _logos = GetComponentsInChildren<Logo>();
        foreach(var logo in _logos)
        {
            Debug.Log(logo.Id);
            // logo.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        RegisterEndpoints();
    }


    void OnDisable()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableLogo");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/disableLogo");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/toggleLogo");
    }

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{OscAddress}/enableLogo", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Enable Logo {value}");
            EnableLogo(value);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/disableLogo", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Disable Logo {value}");
            DisableLogo(value);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/toggleLogo", (OscDataHandle dataHandle) => {
            var value = dataHandle.GetElementAsString(0);
            Debug.Log($"Toggle Logo {value}");
            ToggleLogo(value);
        });
    }

    private void EnableLogo(string id)
    {
        var logo = _logos.FirstOrDefault(l => l.Id.ToLower() == id.ToLower());
        logo.gameObject.SetActive(true);
    }

    private void DisableLogo(string id)
    {
        var logo = _logos.FirstOrDefault(l => l.Id.ToLower() == id.ToLower());
        logo.FadeOut();
    }

    private void ToggleLogo(string id)
    {
        var logo = _logos.FirstOrDefault(l => l.Id.ToLower() == id.ToLower());
        if (logo.gameObject.activeSelf)
        {
            logo.FadeOut();
        }
        else
        {
            logo.gameObject.SetActive(true);
        }
    }
}
