using System;
using UnityEngine;

public static class Texture2DExtensions
{
    public static string ToBase64(this Texture2D texture, string format = "png")
    {
        switch (format)
        {
            case "png":
                return texture.ToBase64Png();
            case "jpg":
                return texture.ToBase64Jpg();
            default:
                throw new ArgumentException($"Unsupported format: {format}");
        }
    }

    public static string ToBase64Png(this Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        return Convert.ToBase64String(bytes);
    }

    public static string ToBase64Jpg(this Texture2D texture)
    {
        byte[] bytes = texture.EncodeToJPG();
        return Convert.ToBase64String(bytes);
    }

    public static Texture2D Duplicate(this Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
