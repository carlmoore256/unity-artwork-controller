public interface IWebSocketClient
{
    bool IsEnabled { get; }
    void Enable();
    void Disable();
}

public interface IWebSocketMessageHandler
{
    void HandleMessage(WebSocketReceiveMessageData messageData);
}

public interface IWebSocketBroadcaster
{
    void Broadcast();
}
