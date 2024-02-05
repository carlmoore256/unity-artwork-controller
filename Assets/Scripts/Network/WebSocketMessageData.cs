using Newtonsoft.Json;
using WebSocketSharp;
using System;
using UnityEngine;

public class __WebSocketReceiveMessageData
{
    public MessageEventArgs Message { get; private set; }

    // Add other relevant properties here

    public __WebSocketReceiveMessageData(MessageEventArgs message)
    {
        Message = message;
    }

    // You can add methods to parse the message into more structured data
}

public class WebSocketReceiveMessageData
{
    public object data;
    public string messageType;

    public WebSocketReceiveMessageData(object data, string messageType)
    {
        this.data = data;
        this.messageType = messageType;
    }

    public static WebSocketReceiveMessageData ParseMessage(string json)
    {
        try {
            var data = JsonConvert.DeserializeObject<WebSocketReceiveMessageData>(json);
            return data;
        } catch (Exception e) {
            Debug.LogError($"Failed to parse message data: {e.Message}");
            return null;
        }
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(new { data, messageType });
    }

    public T Deserialize<T>()
    {
        try {
            return JsonConvert.DeserializeObject<T>(data.ToString());
        } catch (Exception e) {
            Debug.LogError($"Failed to parse message data: {e.Message}");
            return default(T);
        }
    }
}

public class WebSocketSendMessageData
{
    public object data;
    public string messageType;

    public WebSocketSendMessageData(object data, string messageType)
    {
        this.data = data;
        this.messageType = messageType;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(new { data, messageType });
    }
}
