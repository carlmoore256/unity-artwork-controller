using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System;


public class OscEndpoint
{
    private readonly string _address;
    public string Address => _address;
    private List<Action<OscDataHandle>> _listeners = new List<Action<OscDataHandle>>();    
    private readonly OscServer _server;


    public OscEndpoint(string address, OscServer server)
    {
        _address = $"{OscManager.RootAddress}{address}";
        _server = server;
        _server.MessageDispatcher.AddCallback(_address, NotifyListeners);
    }

    public void Deactivate()
    {
        if (_server != null && _server.MessageDispatcher != null) {
            _server.MessageDispatcher.RemoveCallback(_address, NotifyListeners);
        }
    }

    public void AddListener(Action<OscDataHandle> listener)
    {
        _listeners.Add(listener);
    }

    public void RemoveListener(Action<OscDataHandle> listener)
    {
        _listeners.Remove(listener);
    }


    private void NotifyListeners(string address, OscDataHandle data)
    {
        foreach (var listener in _listeners)
        {
            Dispatcher.RunOnMainThread(() => {
                listener?.Invoke(data);
            });
        }
    }

    public override string ToString()
    {
        return "[OscEndpoint at " + _address + " with " + _listeners.Count + " listeners]";
    }
}