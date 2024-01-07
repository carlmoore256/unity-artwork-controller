using WebSocketSharp;

public class WebSocketMessageData
{
    public MessageEventArgs Message { get; private set; }

    // Add other relevant properties here

    public WebSocketMessageData(MessageEventArgs message)
    {
        Message = message;
    }

    // You can add methods to parse the message into more structured data
}