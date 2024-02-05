using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// Custom Attribute Definition, allows us to decorate method with an address
[AttributeUsage(AttributeTargets.Method)]
public class NetworkEndpointAttribute : Attribute
{
    public string Address { get; private set; }
    public EndpointParameters Parameters { get; private set; }

    public NetworkEndpointAttribute(string address, EndpointParameters parameters)
    {
        Address = address;
        Parameters = parameters;
    }
}

// Endpoint Handler Class
public class EndpointHandler
{
    private List<string> _registeredEndpoints = new List<string>();

    // public EndpointHandler(INetworkEndpoint controller, string addressPrefix = null)
    // {
    //     RegisterEndpoints(controller);
    // }

    private string ResolveAddress(string address)
    {
        if (address.StartsWith("/"))
        {
            return address;
        }
        return $"/{address}";
    }

    public void RegisterEndpoint(string address, Action<__WebSocketReceiveMessageData> onMessageReceived)
    {
        address = ResolveAddress(address);
        if (_registeredEndpoints.Contains(address))
        {
            throw new Exception($"Endpoint {address} already registered");
        }
        WebSocketHost.Instance.RegisterEndpoint(address, onMessageReceived);
        _registeredEndpoints.Add(address);
    }

    public void RegisterEndpoints(object controller, string addressPrefix = null)
    {
        var methods = controller
            .GetType()
            .GetMethods()
            .Where(m => m.GetCustomAttributes(typeof(NetworkEndpointAttribute), false).Length > 0);

        foreach (var method in methods)
        {
            var attribute = (NetworkEndpointAttribute)
                method.GetCustomAttributes(typeof(NetworkEndpointAttribute), false).First();
            var address = ResolveAddress(attribute.Address);
            var action =
                (Action<__WebSocketReceiveMessageData>)
                    Delegate.CreateDelegate(
                        typeof(Action<__WebSocketReceiveMessageData>),
                        controller,
                        method
                    );
            WebSocketHost.Instance.RegisterEndpoint(address, action);
            _registeredEndpoints.Add(address);
        }
    }

    public void UnregisterEndpoints()
    {
        _registeredEndpoints.ForEach(address =>
        {
            WebSocketHost.Instance.UnregisterEndpoint(address);
        });
        _registeredEndpoints.Clear();
    }
}
