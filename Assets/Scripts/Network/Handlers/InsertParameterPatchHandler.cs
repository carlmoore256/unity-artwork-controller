using System.Collections.Generic;
using UnityEngine;

public class InsertParameterPatchHandler : IWebSocketMessageHandler
{
    private IInsert _insert;

    // maybe make the input an instance of an insert instead of an artwork
    public InsertParameterPatchHandler(IInsert insert)
    {
        _insert = insert;
    }

    public void HandleMessage(WebSocketReceiveMessageData messageData)
    {
        // the issue is all of these class instances need to deserialize the message data
        // CHANGE THIS!
        var message = messageData.Deserialize<InsertParameterPatchMessage>();
        Debug.Log(
            $"Insert {_insert.Name} {_insert.Id} | message value {message.parameter.value} | message name {message.parameter.name}"
        );
        try
        {
            var parameters = _insert.GetParameters();
            Debug.Log(
                $"Insert {_insert.Name} {_insert.Id} | message value {message.parameter.value} | message name {message.parameter.name}"
            );
            parameters.TryPatchParameter(message.parameter.name, message.parameter.value);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error patching parameter: {e}");
        }
    }

    public class InsertParameterPatchMessage
    {
        public string id;
        public InsertParameterType parameter;
    }

    public class InsertParameterType
    {
        public string name;
        public string Type;
        public object value;
    }
}
