using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public interface IWebSocketSendMessageData<T>
{
    T Value { get; }
    string Type { get; }
}

public class WebSocketHost : MonoBehaviour
{
    public static WebSocketHost Instance { get; private set; }
    public int port = 8080;
    private WebSocketServer wss;

    public Action<string> OnClientConnected;
    public Action<WebSocketReceiveMessageData> OnMessageReceived;
    private Dictionary<string, Action<WebSocketReceiveMessageData>> _listeners = new Dictionary<string, Action<WebSocketReceiveMessageData>>();

    public string Endpoint { get; } = "/Unity";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            Instance = new GameObject("WebSocketHost").AddComponent<WebSocketHost>();
            DontDestroyOnLoad(Instance.gameObject);
        }
    }

    public void AddListener(string messageType, Action<WebSocketReceiveMessageData> listener)
    {
        if (_listeners.ContainsKey(messageType))
        {
            _listeners[messageType] += listener;
            return;
        } else {
            _listeners[messageType] = listener;
        }
    }

    public void RemoveListener(string messageType, Action<WebSocketReceiveMessageData> listener)
    {
        if (_listeners.ContainsKey(messageType))
        {
            _listeners[messageType] -= listener;
        }
    }

    void Start()
    {
        wss = new WebSocketServer(port);
        wss.AddWebSocketService<WebSocketEndpoint>(Endpoint);
        wss.Start();
        Debug.Log("WebSocket Server started on ws://localhost:8080/Unity");
        Instance = this;
        // wss.WebSocketServices[Endpoint]
        //  += (sender, e) => OnOpen();
    }

    public void ClientConnected(string clientId)
    {
        Dispatcher.RunOnMainThread(() =>
        {
            OnClientConnected?.Invoke(clientId);
        });
    }

    public void ClientDisconnected(string clientId)
    {
        Dispatcher.RunOnMainThread(() =>
        {
            // Handle client disconnection
            Debug.Log("Client disconnected: " + clientId);
        });
    }

    public void ClientMessageReceived(string clientId, string message)
    {
        Dispatcher.RunOnMainThread(() =>
        {
            // Debug.Log("Client message received: " + message);
            // Handle client message
            var messageData = WebSocketReceiveMessageData.ParseMessage(message);
            if (_listeners.ContainsKey(messageData.messageType))
            {
                _listeners[messageData.messageType]?.Invoke(messageData);
            }
        });
    }

    void OnDestroy()
    {
        if (wss != null)
        {
            wss.Stop();
            wss = null;
        }
    }

    public void RegisterEndpoint(
        string address,
        Action<__WebSocketReceiveMessageData> onMessageReceived
    )
    {
        // var service = wss.WebSocketServices["/Unity"];
        // var webSocketService = (WebSocketService)service;
        // webSocketService.OnMessageReceived = onMessageReceived;
    }

    public void UnregisterEndpoint(string address)
    {
        wss.RemoveWebSocketService(address);
    }

    public void Broadcast(string message)
    {
        wss.WebSocketServices[Endpoint].Sessions.Broadcast(message);
    }

    public void Broadcast(object data)
    {
        string json = JsonConvert.SerializeObject(data);
        wss.WebSocketServices[Endpoint].Sessions.Broadcast(json);
    }

    public void Broadcast(WebSocketSendMessageData data)
    {
        wss.WebSocketServices[Endpoint].Sessions.Broadcast(data.ToJson());
    }
}

// idea - the controllers expose endpoints, which can inform the client
// of what kinds of controls are available

// controllers can have a service, it could be generic or it could be more
// specific

// letting the client know what configuration is available is good, because
// different types of art will have different interfaces available
// basically, Unity will let the front end know what it expects in terms
// of value ranges

// there should be a specific endpoint that lets the client know of new things
// added to the endpoints

// if /endpoints gave a list of objects, that all contained metadata about the
// endpoint, that would be good. Remember we need to think about a parent containing
// a set of endpoints. That parent has some properties. For instance, an artwork
// has a name, photo, and status of whether it is active or not. Well, maybe not the
// status. The global artwork controller could be in charge of sending info about
// what's available as endpoint metadata.

public class WebSocketEndpoint : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e)
    {
        WebSocketHost.Instance.ClientMessageReceived(ID, e.Data);
    }

    protected override void OnOpen()
    {
        WebSocketHost.Instance.ClientConnected(ID);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        WebSocketHost.Instance.ClientDisconnected(ID);
    }

    public void SendMessageToClient(string message)
    {
        Send(message);
    }
}
