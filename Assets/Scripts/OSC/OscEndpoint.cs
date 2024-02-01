using System;
using System.Collections;
using System.Collections.Generic;
using OscJack;
using UnityEngine;

public class OscEndpoint
{
    private readonly string _address;
    public string Address => _address;
    public object Owner => _owner;
    private List<Action<OscDataHandle>> _listeners = new List<Action<OscDataHandle>>();
    private readonly OscServer _server;
    private object _owner;

    public OscEndpoint(string address, OscServer server)
    {
        if (!address.StartsWith("/"))
        {
            address = $"/{address}";
        }
        _address = $"{OscManager.RootAddress}{address}";
        _server = server;
        _server.MessageDispatcher.AddCallback(_address, NotifyListeners);
    }

    public OscEndpoint(string address, OscServer server, object owner)
        : this(address, server)
    {
        _owner = owner;
    }

    public void Deactivate()
    {
        if (_server != null && _server.MessageDispatcher != null)
        {
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
            Dispatcher.RunOnMainThread(() =>
            {
                listener?.Invoke(data);
            });
        }
    }

    public override string ToString()
    {
        return "[OscEndpoint at " + _address + " with " + _listeners.Count + " listeners]";
    }
}
