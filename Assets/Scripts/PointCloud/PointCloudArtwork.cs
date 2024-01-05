using System.Collections.Generic;
using Pcx;
using UnityEngine;
using UnityEngine.UI;

public class VolumetricPixel
{
    public Vector3 position;
    public Color color;
    public float depth;
    public int segmentation;
}

public class PointCloudMotif
{
    public string name;
    public List<VolumetricPixel> pixels;
}

public class PointCloudArtwork
{
    public string artworkName;
    private Texture2D _colorTexture;
    private Texture2D _depthTexture;
    private Texture2D _segmentationTexture;
    public VolumetricPixel[] Pixels { get; private set; }
    public float pointDensity = 0.1f;

    public PointCloudArtwork(string artworkName, float pointDensity = 0.1f)
    {
        this.artworkName = artworkName;
        this.pointDensity = pointDensity;
        LoadArtworkFromImages(artworkName);
    }

    public List<Vector3> GetPositions(Vector3 scalar)
    {
        // PointCloudRenderer
        List<Vector3> positions = new List<Vector3>();
        foreach (VolumetricPixel pixel in Pixels)
        {
            positions.Add(Vector3.Scale(pixel.position, scalar));
        }
        return positions;
    }

    public List<Color> GetColors()
    {
        List<Color> colors = new List<Color>();
        foreach (VolumetricPixel pixel in Pixels)
        {
            colors.Add(pixel.color);
        }
        return colors;
    }

    public void LoadArtworkFromImages(string artworkName)
    {
        // Load textures from assets
        _colorTexture = Resources.Load<Texture2D>("PointClouds/" + artworkName + "/color");
        _depthTexture = Resources.Load<Texture2D>("PointClouds/" + artworkName + "/depth");
        _segmentationTexture = Resources.Load<Texture2D>(
            "PointClouds/" + artworkName + "/segments"
        );

        // Make sure they are all the same size
        if (
            _colorTexture.width != _depthTexture.width
            || _colorTexture.width != _segmentationTexture.width
            || _colorTexture.height != _depthTexture.height
            || _colorTexture.height != _segmentationTexture.height
        )
        {
            Debug.Log("Color: " + _colorTexture.width + "x" + _colorTexture.height);
            Debug.Log("Depth: " + _depthTexture.width + "x" + _depthTexture.height);
            Debug.Log(
                "Segmentation: " + _segmentationTexture.width + "x" + _segmentationTexture.height
            );
            Debug.LogError("Textures are not the same size!");
            return;
        }

        var allPixels = CreateVolumetricPixels();
        Pixels = RandomlySamplePixels(allPixels, this.pointDensity);
    }

    private VolumetricPixel[] CreateVolumetricPixels()
    {
        int width = _colorTexture.width;
        int height = _colorTexture.height;
        var allPixels = new VolumetricPixel[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color colorPixel = _colorTexture.GetPixel(x, y);
                float depthPixel = _depthTexture.GetPixel(x, y).GetBrightness(); // Assuming depth is stored in the red channel
                int segmentationPixel = GetSegmentation(_segmentationTexture.GetPixel(x, y));
                VolumetricPixel vp = new VolumetricPixel
                {
                    color = colorPixel,
                    depth = depthPixel,
                    segmentation = segmentationPixel,
                    position = new Vector3(x, y, depthPixel * 1000) // Modify this based on how you want to interpret depth
                };

                allPixels[y * width + x] = vp;
            }
        }
        return allPixels;
    }

    private VolumetricPixel[] RandomlySamplePixels(VolumetricPixel[] pixels, float sampleRatio)
    {
        int numPixels = (int)(pixels.Length * sampleRatio);
        var sampledPixels = new VolumetricPixel[numPixels];

        // Random sampling
        System.Random random = new System.Random();
        HashSet<int> selectedIndices = new HashSet<int>();

        while (selectedIndices.Count < numPixels)
        {
            int index = random.Next(pixels.Length);
            selectedIndices.Add(index);
        }

        int i = 0;
        foreach (int index in selectedIndices)
        {
            sampledPixels[i++] = pixels[index];
        }

        return sampledPixels;
    }

    private int GetSegmentation(Color color)
    {
        int r = Mathf.FloorToInt(color.r * 255);
        int g = Mathf.FloorToInt(color.g * 255);
        int b = Mathf.FloorToInt(color.b * 255);

        return r * 256 * 256 + g * 256 + b;
    }
}
