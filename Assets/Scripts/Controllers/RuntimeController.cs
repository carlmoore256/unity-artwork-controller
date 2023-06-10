using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using UnityEngine.SceneManagement;

public class RuntimeController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/runtime";


    private void OnEnable()
    {
        RegisterEndpoints();
    }

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{OscAddress}/quit", (OscDataHandle dataHandle) => {
            Debug.Log("Quitting");
            // Application.Quit();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/reload", (OscDataHandle dataHandle) => {
            Debug.Log("Reloading Scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
