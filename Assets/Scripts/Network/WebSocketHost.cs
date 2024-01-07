using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebSocketHost : MonoBehaviour
{
    public static WebSocketHost Instance { get; private set; }
    public int port = 8080;
    private WebSocketServer wss;

    void Start()
    {
        wss = new WebSocketServer(port);
        wss.AddWebSocketService<WebSocketEndpoint>("/Unity");
        wss.Start();
        Debug.Log("WebSocket Server started on ws://localhost:8080/Unity");
        Instance = this;
    }

    void OnDestroy()
    {
        if (wss != null)
        {
            wss.Stop();
            wss = null;
        }
    }

    public void RegisterEndpoint(string address, Action<WebSocketMessageData> onMessageReceived)
    {
        // var service = wss.WebSocketServices["/Unity"];
        // var webSocketService = (WebSocketService)service;
        // webSocketService.OnMessageReceived = onMessageReceived;
    }

    public void UnregisterEndpoint(string address)
    {
        wss.RemoveWebSocketService(address);
    }
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            wss.WebSocketServices["/Unity"].Sessions.Broadcast($"Hello from Unity!");
        }
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
        Debug.Log("Received message from client: " + e.Data);
        Send("Echo: " + e.Data);
    }

    protected override void OnOpen()
    {
        // on open will be where we send the client the configuration
        // options
        Debug.Log("Client connected: " + ID);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Send("Goodbye");
        Debug.Log("Client disconnected: " + ID);
    }

    public void SendMessageToClient(string message)
    {
        Send(message);
    }
}
