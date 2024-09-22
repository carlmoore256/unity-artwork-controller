using UnityEngine;

public class ArtworkToggleHandler : IWebSocketMessageHandler
{
    private ArtworkSceneController _artworkSceneController;

    public ArtworkToggleHandler(ArtworkSceneController artworkSceneController)
    {
        _artworkSceneController = artworkSceneController;
    }

    public void HandleMessage(WebSocketReceiveMessageData messageData)
    {
        var message = messageData.Deserialize<ToggleArtworkMessage>();
        var artwork = ArtworkLoader.Instance.GetArtwork(message.id);
        Debug.Log("Toggling artwork " + message.id);
        if (artwork != null)
        {
            _artworkSceneController.ToggleArtwork(artwork);
        }
    }

    public class ToggleArtworkMessage
    {
        public string id;
    }
}
