public class ArtworkInfoBroadcaster : IWebSocketBroadcaster
{
    private IArtwork _artwork;
    private ArtworkSceneController _artworkSceneController;
    public ArtworkInfoBroadcaster(IArtwork artwork, ArtworkSceneController artworkSceneController)
    {
        _artwork = artwork;
        _artworkSceneController = artworkSceneController;
    }

    public void Broadcast()
    {
        var thumbnailTexture = ArtworkLoader.Instance.GetArtworkThumbnail(_artwork);
        string thumbnailString = thumbnailTexture != null ? thumbnailTexture.ToBase64() : null;
        bool enabled = _artworkSceneController.IsArtworkEnabled(_artwork);
        WebSocketHost.Instance.Broadcast(
            new WebSocketSendMessageData(
                new
                {
                    metadata = _artwork.GetMetadata(),
                    thumbnail = thumbnailString,
                    enabled
                },
                "artwork-info"
            )
        );
    }
}
