using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using UnityEngine.SceneManagement;

public class RuntimeController : MonoBehaviour, INetworkEndpoint
{
    public string Address => "/runtime";


    private bool _reset = false;


    private void OnEnable()
    {
        Register("/runtime");
    }

    private void OnDisable()
    {
        Unregister();
    }

    private void OnDestroy()
    {
        Unregister();
    }

    public void Register(string baseAddress)
    {
        // OscManager.Instance.AddEndpoint($"{OscAddress}/quit", (OscDataHandle dataHandle) => {
        //     Debug.Log("Quitting");
        //     // Application.Quit();
        // });

        OscManager.Instance.AddStaticEndpoint($"{baseAddress}/reload", (OscDataHandle dataHandle) => {
            _reset = true;
        });
    }

    public void Unregister()
    {
        // if (OscManager.Instance == null) return;
        // OscManager.Instance.RemoveEndpoint($"{Address}/quit");
        // OscManager.Instance.RemoveEndpoint($"{Address}/reload");
    }

    // Update is called once per frame
    void Update()
    {
        if (_reset)
        {
            _reset = false;
            OscManager.Instance.ResetEndpoints();
            Dispatcher.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }        
    }
}
