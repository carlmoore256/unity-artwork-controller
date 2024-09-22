using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// public class ArtworkInsertParameterBroadcaster : IWebSocketBroadcaster
// {
//     public string MessageType = "insert-info";

//     private IArtwork _artwork;
//     private IInsert[] _inserts;

//     public ArtworkInsertParameterBroadcaster(IArtwork artwork)
//     {
//         _artwork = artwork;
//         _inserts = _artwork.GetInserts();
//     }

//     public void Broadcast()
//     {
//         foreach (var insert in _inserts)
//         {
//             var parameters = insert.GetParameters();
//             var insertData = new
//             {
//                 artworkId = _artwork.Id,
//                 insertId = insert.Id,
//                 name = insert.Name,
//                 parameters
//             };

//             WebSocketHost.Instance.Broadcast(new WebSocketSendMessageData(insertData, MessageType));
//         }
//     }
// }

// this thing should also implement some sort of interface
public class ArtworkInsertParameterHandler : IWebSocketMessageHandler
{
    public bool IsEnabled { get; private set; }
    private IArtwork _artwork;
    private Dictionary<string, IInsert> _inserts = new Dictionary<string, IInsert>();

    public ArtworkInsertParameterHandler(IArtwork artwork)
    {
        _artwork = artwork;
        var allInserts = _artwork.GetInserts();
        foreach (var insert in allInserts)
        {
            _inserts.Add(insert.Id, insert);
        }
    }

    // public void BroadcastInsertInfo()
    // {
    //     var allInserts = _artwork.GetInserts();
    //     foreach (var insert in allInserts)
    //     {
    //         _inserts.Add(insert.Id, insert);
    //     }
    //     foreach (var insert in _inserts)
    //     {
    //         var parameters = insert.Value.GetParameters();
    //         var insertData = new
    //         {
    //             artworkId = _artwork.Id,
    //             insertId = insert.Value.Id,
    //             name = insert.Value.Name,
    //             parameters
    //         };

    //         WebSocketHost.Instance.Broadcast(
    //             new WebSocketSendMessageData(insertData, "insert-info")
    //         );
    //     }
    // }

    // in general, this could be an interface that handles messages to/from an object
    // and could be an IWebSocketListener or something

    // public void Enable()
    // {
    //     WebSocketHost.Instance.AddListener("insert-patch", OnInsertParameterPatch);
    //     IsEnabled = true;
    // }

    // public void Disable()
    // {
    //     WebSocketHost.Instance.RemoveListener("insert-patch", OnInsertParameterPatch);
    //     IsEnabled = false;
    // }

    public void HandleMessage(WebSocketReceiveMessageData messageData)
    {
        var message = messageData.Deserialize<InsertParameterPatchMessage>();
        // Debug.Log($"Am I active? {gameObject.activeInHierarchy} | {gameObject.activeSelf}");
        if (_artwork.Id != message.artworkId)
        {
            return;
        }
        if (!_inserts.ContainsKey(message.insertId))
        {
            Debug.LogError($"Could not find insert with id {message.insertId}");
            return;
        }
        var insert = _inserts[message.insertId];
        if (insert != null)
        {
            try
            {
                var parameters = insert.GetParameters();
                Debug.Log(
                    $"Insert {insert.Name} | message value {message.parameter.value} | message name {message.parameter.name}"
                );
                parameters.TryPatchParameter(message.parameter.name, message.parameter.value);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error patching parameter: {e}");
            }
        }
    }

    public class InsertParameterPatchMessage
    {
        public string artworkId;
        public string insertId;
        public InsertParameterType parameter;
    }

    public class InsertParameterType
    {
        public string name;
        public string Type;
        public object value;
    }
}


// var insertData = new
// {
//     artworkId = artwork.Id,
//     insertId = insert.Id,
//     name = insert.Name,
//     parameters
// };
// WebSocketHost.Instance.Broadcast(
//     new WebSocketSendMessageData(insertData, "insert-info")
// );
// WebSocketHost.Instance.AddListener("insert-patch", OnInsertParameterPatch);


// private void OnEnable()
// {
//     _inserts = new Dictionary<string, IInsert>();
//     _artwork = GetComponent<IArtwork>();

//     var allInserts = gameObject.GetComponents<IInsert>();
//     Debug.Log($"Found {allInserts.Length} inserts");
//     foreach (var insert in allInserts)
//     {
//         Debug.Log($"Insert: {insert.Name} ({insert.Id})");
//         var parameters = insert.GetParameters();

//         _inserts.Add(insert.Id, insert);

//         var insertData = new
//         {
//             artworkId = _artwork.Id,
//             insertId = insert.Id,
//             name = insert.Name,
//             parameters
//         };

//         WebSocketHost.Instance.Broadcast(
//             new WebSocketSendMessageData(insertData, "insert-info")
//         );

//         WebSocketHost.Instance.AddListener("insert-patch", OnInsertParameterPatch);

//         // we have to remove the IArtwork after destroying the object
//     }
// }

// private void OnDisable()
// {
//     WebSocketHost.Instance.RemoveListener("insert-patch", OnInsertParameterPatch);
// }

// private void OnDestroy()
// {
//     // how are we supposed to know when something has been destroyed?
//     WebSocketHost.Instance.RemoveListener("insert-patch", OnInsertParameterPatch);
// }
