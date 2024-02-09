using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

// this should implement some sort of interface that is common for
// websocket handlers
[RequireComponent(typeof(ArtworkSceneController))]
public class SceneWebsocketHandler : MonoBehaviour
{
    private ArtworkSceneController _artworkSceneController;

    private void Awake()
    {
        _artworkSceneController = GetComponent<ArtworkSceneController>();
    }

    private void OnEnable()
    {
        _artworkSceneController.OnArtworkEnabled += OnArtworkEnabled;
        _artworkSceneController.OnArtworkDisabled += OnArtworkDisabled;
        WebSocketHost.Instance.OnClientConnected += OnClientConnected;
        WebSocketHost.Instance.AddListener("toggle-artwork", OnToggleArtwork);
    }

    private void OnDisable()
    {
        _artworkSceneController.OnArtworkEnabled -= OnArtworkEnabled;
        _artworkSceneController.OnArtworkDisabled -= OnArtworkDisabled;
        WebSocketHost.Instance.OnClientConnected -= OnClientConnected;
        WebSocketHost.Instance.RemoveListener("toggle-artwork", OnToggleArtwork);
    }

    private void OnClientConnected(string clientId)
    {
        Debug.Log("Client connected");
        BroadcastArtworksAvailable();
    }

    private void OnToggleArtwork(WebSocketReceiveMessageData messageData)
    {
        var message = messageData.Deserialize<ToggleArtworkMessage>();
        var artwork = ArtworkLoader.Instance.GetArtwork(message.id);
        Debug.Log("Toggling artwork " + message.id);
        if (artwork != null)
        {
            _artworkSceneController.ToggleArtwork(artwork);
        }
    }

    public void BroadcastArtworksAvailable()
    {
        foreach (var artwork in ArtworkLoader.Instance.AvailableArtworks)
        {
            var thumbnailTexture = ArtworkLoader.Instance.GetArtworkThumbnail(artwork);
            string thumbnailString = thumbnailTexture != null ? thumbnailTexture.ToBase64() : null;
            bool enabled = _artworkSceneController.IsArtworkEnabled(artwork);
            WebSocketHost.Instance.Broadcast(
                new WebSocketSendMessageData(
                    new
                    {
                        metadata = artwork.GetMetadata(),
                        thumbnail = thumbnailString,
                        enabled
                    },
                    "artwork-available"
                )
            );
        }
    }

    private void OnArtworkEnabled(ArtworkMetadata metadata)
    {
        WebSocketHost.Instance.Broadcast(
            new WebSocketSendMessageData(new { metadata, enabled = true }, "toggle-artwork")
        );
    }

    private void OnArtworkDisabled(ArtworkMetadata metadata)
    {
        WebSocketHost.Instance.Broadcast(
            new WebSocketSendMessageData(new { metadata, enabled = false }, "toggle-artwork")
        );
    }

    public class ToggleArtworkMessage
    {
        public string id;
    }
}
