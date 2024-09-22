using UnityEngine;

[RequireComponent(typeof(FpsCounter))]
public class SceneStatsBroadcaster : MonoBehaviour, IWebSocketBroadcaster
{
    public float broadcastFrequency = 1.0f;
    private FpsCounter _fpsCounter;

    private void OnEnable()
    {
        _fpsCounter = GetComponent<FpsCounter>();
        InvokeRepeating(nameof(Broadcast), 0, broadcastFrequency);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Broadcast));
    }

    public void Broadcast()
    {
        WebSocketHost.Instance.Broadcast(
            new WebSocketSendMessageData(new { fps = _fpsCounter.FrameRate }, "scene-stats")
        );
    }

    private int GetNumSceneObjects()
    {
        return FindObjectsOfType<GameObject>().Length;
    }
    
}
