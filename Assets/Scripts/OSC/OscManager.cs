using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System;


public class OscManager : MonoBehaviour
{
    private static OscManager _instance;
    public static OscManager Instance { get {
        if (_instance == null) {
            _instance = FindObjectOfType<OscManager>();
        }
        return _instance;
    } }
     
    public ScriptableObject oscSettings;

    public static string RootAddress { get; } = "/unity";

    public int Port = 8000;

    private OscServer _server;
    public OscServer Server { get {
        if (_server == null) {
            _server = new OscServer(Port);
        }
        return _server;
    } }

    private List<OscEndpoint> _endpoints = new List<OscEndpoint>();

    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         _instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }

    //     _server = new OscServer(Port);
    // }

    public OscEndpoint AddEndpoint(string address)
    {
        var newEndpoint = new OscEndpoint(address, Server);
        _endpoints.Add(newEndpoint);
        return newEndpoint;
    }

    public OscEndpoint AddEndpoint(string address, Action<OscDataHandle> listener)
    {
        var newEndpoint = new OscEndpoint(address, Server);
        newEndpoint.AddListener(listener);
        _endpoints.Add(newEndpoint);
        return newEndpoint;
    }


    private void OnApplicationQuit()
    {
        DisposeServer();
    }

    private void OnDisable()
    {
        DisposeServer();
    }

    private void DisposeServer() { _server?.Dispose(); }
}
