
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class StoredEchoEffect : MonoBehaviour
{
    public Material effectMaterial;
    public float echoIntensity = 0.5f;
    private RenderTexture[] prevFrames;
    private int currentIndex = 0;
    public int echoDelayFrames = 10; // Number of frames to wait before showing the echo
    public int echoStoredFrames = 5; // Number of echoes to show

    private void Start()
    {
        prevFrames = new RenderTexture[echoStoredFrames];
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        currentIndex = (currentIndex + 1) % (echoDelayFrames * echoStoredFrames);
        
        int index = currentIndex / echoDelayFrames;
        
        if (prevFrames[index] == null || prevFrames[index].width != source.width || prevFrames[index].height != source.height)
        {
            if (prevFrames[index] != null)
            {
                DestroyImmediate(prevFrames[index]);
            }

            prevFrames[index] = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            prevFrames[index].hideFlags = HideFlags.HideAndDontSave;
        }

        if (currentIndex % echoDelayFrames == 0)
        {
            Graphics.Blit(source, prevFrames[index]);
        }

        effectMaterial.SetTexture("_PrevTex", prevFrames[index]);
        effectMaterial.SetFloat("_EchoIntensity", echoIntensity);

        Graphics.Blit(source, destination, effectMaterial);
    }

    private void OnDisable()
    {
        for (int i = 0; i < echoStoredFrames; i++)
        {
            if (prevFrames[i] != null)
            {
                DestroyImmediate(prevFrames[i]);
                prevFrames[i] = null;
            }
        }
    }
}