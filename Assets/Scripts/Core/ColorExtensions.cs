using UnityEngine;

public static class ColorExtensions
{
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static Color WithAlpha(this Color color, int alpha)
    {
        return new Color(color.r, color.g, color.b, alpha / 255f);
    }

    public static float GetBrightness(this Color color)
    {
        return Mathf.Max(color.r, color.g, color.b);
    }

    /// <summary>
    /// Returns a new color with a rotation of the hue
    /// </summary>
    public static Color WithHueRotation(this Color color, float offset)
    {
        float H,
            S,
            V;
        Color.RGBToHSV(color, out H, out S, out V);
        S = Mathf.Cos((offset * Mathf.PI * 2f) + Mathf.PI) * 0.5f + 0.5f;
        H += offset;
        H = H % 1f;
        return Color.HSVToRGB(H, S, V);
    }

    public static Color WithHue(this Color color, float hue)
    {
        float H,
            S,
            V;
        Color.RGBToHSV(color, out H, out S, out V);
        return Color.HSVToRGB(hue, S, V);
    }

    public static Color WithSaturation(this Color color, float saturation)
    {
        float H,
            S,
            V;
        Color.RGBToHSV(color, out H, out S, out V);
        return Color.HSVToRGB(H, saturation, V);
    }

    public static Color WithValue(this Color color, float value)
    {
        float H,
            S,
            V;
        Color.RGBToHSV(color, out H, out S, out V);
        return Color.HSVToRGB(H, S, value);
    }
}
