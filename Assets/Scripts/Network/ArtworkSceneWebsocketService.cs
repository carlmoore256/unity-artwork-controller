using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// things boil down to a collection of broadcasters and listeners (handlers)
// this should implement some sort of interface that is common for
// websocket handlers
[RequireComponent(typeof(ArtworkSceneController))]
public class ArtworkSceneWebsocketService : MonoBehaviour
{
    private ArtworkSceneController _artworkSceneController;

    private ArtworkToggleHandler _toggleHandler;

    private InsertParameterPatchHandler _insertParameterPatchHandler;
    private Dictionary<string, IWebSocketMessageHandler> _parameterPatchHandlers =
        new Dictionary<string, IWebSocketMessageHandler>();

    private void Awake()
    {
        _artworkSceneController = GetComponent<ArtworkSceneController>();
        _toggleHandler = new ArtworkToggleHandler(_artworkSceneController);
    }

    private void OnEnable()
    {
        _artworkSceneController.OnArtworkEnabled += OnArtworkEnabled;
        _artworkSceneController.OnArtworkDisabled += OnArtworkDisabled;
        WebSocketHost.Instance.OnClientConnected += OnClientConnected;
        WebSocketHost.Instance.RegisterMessageHandler("artwork-toggle", _toggleHandler);

        // foreach (var artwork in _artworkSceneController.AvailableArtworks)
        // {
        //     var inserts = artwork.GetInserts();
        //     foreach (var insert in inserts)
        //     {
        //         var handler = new InsertParameterPatchHandler(insert);
        //         _parameterPatchHandlers[insert.Id] = handler;
        //         WebSocketHost.Instance.RegisterMessageHandler($"insert-patch-{insert.Id}", handler);
        //     }
        // }
    }

    private void OnDisable()
    {
        _artworkSceneController.OnArtworkEnabled -= OnArtworkEnabled;
        _artworkSceneController.OnArtworkDisabled -= OnArtworkDisabled;
        WebSocketHost.Instance.OnClientConnected -= OnClientConnected;
        // WebSocketHost.Instance.RemoveListener("toggle-artwork", OnToggleArtwork);

        WebSocketHost.Instance.UnregisterMessageHandler("artwork-toggle", _toggleHandler);

        foreach (var artwork in _artworkSceneController.AvailableArtworks)
        {
            var inserts = artwork.GetInserts();
            foreach (var insert in inserts)
            {
                if (!_parameterPatchHandlers.ContainsKey(insert.Id))
                {
                    Debug.LogWarning(
                        $"No handler for insert {insert.Id} when disabling websocket service, even though it should have been registered."
                    );
                    continue;
                }
                var handler = _parameterPatchHandlers[insert.Id];
                WebSocketHost.Instance.UnregisterMessageHandler(
                    $"insert-patch-{insert.Id}",
                    handler
                );
            }
        }
    }

    private void OnClientConnected(string clientId)
    {
        Debug.Log("Client connected");
        foreach (var artwork in _artworkSceneController.AvailableArtworks)
        {
            artwork.BroadcastInfo(_artworkSceneController.IsArtworkEnabled(artwork));
            // artwork.BroadcastInserts();
        }
    }

    private void OnArtworkEnabled(ArtworkMetadata metadata)
    {
        WebSocketHost.Instance.Broadcast(
            new WebSocketSendMessageData(new { id = metadata.id, enabled = true }, "artwork-toggle")
        );


        // Debug.Log($"Number of active artworks: {_artworkSceneController.ActiveArtworks.Count()}");

        var artwork = _artworkSceneController.ActiveArtworks.FirstOrDefault(
            x => x.Id == metadata.id
        );
        

        artwork.BroadcastInfo(true);

        if (artwork == null)
        {
            Debug.LogWarning($"Artwork {metadata.id} not found when enabling");
            return;
        }
        var inserts = artwork.GetInserts();
        foreach (var insert in inserts)
        {
            var handler = new InsertParameterPatchHandler(insert);
            if (_parameterPatchHandlers.ContainsKey(insert.Id))
            {
                Debug.LogWarning(
                    $"Handler for insert {insert.Id} already exists when enabling artwork, setting new handler."
                );
                continue;
            }
            _parameterPatchHandlers[insert.Id] = handler;
            WebSocketHost.Instance.RegisterMessageHandler($"insert-patch-{insert.Id}", handler);
        }
    }

    private void OnArtworkDisabled(ArtworkMetadata metadata)
    {
        var artwork = _artworkSceneController.ActiveArtworks.FirstOrDefault(
            x => x.Id == metadata.id
        );
        if (artwork == null)
        {
            Debug.LogWarning($"Artwork {metadata.id} not found when disabling");
            return;
        }

        var inserts = artwork.GetInserts();
        foreach (var insert in inserts)
        {
            if (!_parameterPatchHandlers.ContainsKey(insert.Id))
            {
                Debug.LogWarning(
                    $"No handler for insert {insert.Id} when disabling artwork, even though it should have been registered."
                );
                continue;
            }
            var handler = _parameterPatchHandlers[insert.Id];
            WebSocketHost.Instance.UnregisterMessageHandler($"insert-patch-{insert.Id}", handler);

            _parameterPatchHandlers.Remove(insert.Id);
        }

        WebSocketHost.Instance.Broadcast(
            new WebSocketSendMessageData(
                new { id = metadata.id, enabled = false },
                "artwork-toggle"
            )
        );
    }
}



// public void BroadcastArtworksAvailable()
// {
//     foreach (var artwork in ArtworkLoader.Instance.AvailableArtworks)
//     {
//         var thumbnailTexture = ArtworkLoader.Instance.GetArtworkThumbnail(artwork);
//         string thumbnailString = thumbnailTexture != null ? thumbnailTexture.ToBase64() : null;
//         bool enabled = _artworkSceneController.IsArtworkEnabled(artwork);
//         WebSocketHost.Instance.Broadcast(
//             new WebSocketSendMessageData(
//                 new
//                 {
//                     metadata = artwork.GetMetadata(),
//                     thumbnail = thumbnailString,
//                     enabled
//                 },
//                 "artwork-available"
//             )
//         );
//     }
// }
