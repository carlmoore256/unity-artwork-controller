using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OscJack;
using UnityEngine;

public class OscManager : MonoBehaviour
{
    private static OscManager _instance;
    public static OscManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OscManager>();
            }
            return _instance;
        }
    }

    public ScriptableObject oscSettings;

    public static string RootAddress { get; } = "/unity";

    public int Port = 8000;

    private OscServer _server;
    public OscServer Server
    {
        get
        {
            if (_server == null)
            {
                _server = new OscServer(Port);
            }
            return _server;
        }
    }

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


    public void ResetEndpoints()
    {
        _endpoints = new List<OscEndpoint>();
    }

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

    public OscEndpoint AddEndpoint(string address, Action<OscDataHandle> listener, object owner)
    {
        var newEndpoint = new OscEndpoint(address, Server, owner);
        newEndpoint.AddListener(listener);
        _endpoints.Add(newEndpoint);
        return newEndpoint;
    }

    public void RemoveEndpoint(string address)
    {
        address = $"{RootAddress}{address}";
        var endpoint = _endpoints.FirstOrDefault(x => x.Address == address);
        // Debug.Log("Endpoint: " + endpoint);
        if (endpoint != null)
        {
            endpoint.Deactivate();
            _endpoints.Remove(endpoint);
        }
        else
        {
            Debug.LogWarning($"No endpoint found at address {address}");
        }

    }

    public void RemoveEndpoint(OscEndpoint endpoint)
    {
        endpoint.Deactivate();
        _endpoints.Remove(endpoint);
    }

    public void RemoveAllEndpointsForOwner(object owner)
    {
        Debug.Log($"Removing all endpoints for owner {owner}");
        if (owner == null)
        {
            Debug.LogWarning("Owner is null");
            return;
        }
        var endpoints = _endpoints.Where(x => x.Owner == owner).ToArray();
        if (endpoints.Length == 0)
        {
            Debug.LogWarning($"No endpoints found for owner {owner}");
            return;
        }

        foreach (var endpoint in endpoints)
        {
            RemoveEndpoint(endpoint);
        }

        Debug.Log($"Endpoints Remaining: {_endpoints.Count}");
    }

    // option to add an endpoint that is unaffected by the subscriber system
    public void AddStaticEndpoint(string address, Action<OscDataHandle> listener)
    {
        Server.MessageDispatcher.AddCallback(
            $"{RootAddress}{address}",
            (string msgAddress, OscDataHandle data) =>
            {
                listener?.Invoke(data);
            }
        );
    }

    private void OnApplicationQuit()
    {
        DisposeServer();
    }

    private void OnDisable()
    {
        DisposeServer();
    }

    private void DisposeServer()
    {
        _server?.Dispose();
    }

    // void Update()
    // {
    //     foreach(var endpoint in _endpoints)
    //     {
    //         Debug.Log($"Endpoint: {endpoint}");
    //     }
    // }
}
