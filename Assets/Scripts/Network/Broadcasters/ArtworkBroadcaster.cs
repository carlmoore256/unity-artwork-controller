using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;


public static class ArtworkBroadcaster
{
    public static void BroadcastInfo(
        this IArtwork artwork,
        bool isEnabled = false,
        string messageType = "artwork-info"
    )
    {
        var thumbnailTexture = ArtworkLoader.Instance.GetArtworkThumbnail(artwork);
        string thumbnailString = thumbnailTexture != null ? thumbnailTexture.ToBase64() : null;

        var inserts = artwork.GetInserts();
        var insertData = new List<InsertMetadata>();
        foreach (var insert in inserts)
        {
            UnityEngine.Debug.Log($"BROADCASTING Insert {insert.Id} | {insert.Name}");
            insertData.Add(
                new InsertMetadata
                {
                    id = insert.Id,
                    name = insert.Name,
                    parameters = insert.GetParameters()
                }
            );
        }

        WebSocketHost.Instance.Broadcast(
            new WebSocketSendMessageData(
                new
                {
                    metadata = artwork.GetMetadata(),
                    thumbnail = thumbnailString,
                    inserts = insertData,
                    isEnabled,
                },
                messageType
            )
        );
    }

    public static void BroadcastInserts(this IArtwork artwork, string messageType = "insert-info")
    {
        UnityEngine.Debug.Log($"Broadcasting inserts for {artwork.Id}");
        var inserts = artwork.GetInserts();
        foreach (var insert in inserts)
        {
            insert.BroadcastInsert();
        }
    }

    public static void BroadcastInsert(this IInsert insert)
    {
        UnityEngine.Debug.Log($"Broadcasting insert {insert.Id}");
        var parameters = insert.GetParameters();
        var insertData = new
        {
            id = insert.Id,
            name = insert.Name,
            parameters
        };
        WebSocketHost.Instance.Broadcast(new WebSocketSendMessageData(insertData, "insert-info"));
    }
}
