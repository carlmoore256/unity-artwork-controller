using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// this thing should also implement some sort of interface
[RequireComponent(typeof(IArtwork))]
public class InsertParameterRouter : MonoBehaviour
{
    private IArtwork _artwork;
    private Dictionary<string, IInsert> _inserts;

    private void OnEnable()
    {
        _inserts = new Dictionary<string, IInsert>();
        _artwork = GetComponent<IArtwork>();

        var allInserts = gameObject.GetComponents<IInsert>();
        Debug.Log($"Found {allInserts.Length} inserts");
        foreach (var insert in allInserts)
        {
            Debug.Log($"Insert: {insert.Name} ({insert.Id})");
            var parameters = insert.GetParameters();

            _inserts.Add(insert.Id, insert);

            var insertData = new
            {
                artworkId = _artwork.Id,
                insertId = insert.Id,
                name = insert.Name,
                parameters
            };

            WebSocketHost.Instance.Broadcast(
                new WebSocketSendMessageData(insertData, "insert-info")
            );

            WebSocketHost.Instance.AddListener("insert-patch", OnInsertParameterPatch);

            // we have to remove the IArtwork after destroying the object
        }
    }

    private void OnDisable()
    {
        WebSocketHost.Instance.RemoveListener("insert-patch", OnInsertParameterPatch);
    }

    private void OnDestroy()
    {
        WebSocketHost.Instance.RemoveListener("insert-patch", OnInsertParameterPatch);
    }

    private void OnInsertParameterPatch(WebSocketReceiveMessageData messageData)
    {
        var message = messageData.Deserialize<InsertParameterPatchMessage>();
        Debug.Log($"Am I active? {gameObject.activeInHierarchy} | {gameObject.activeSelf}");
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
