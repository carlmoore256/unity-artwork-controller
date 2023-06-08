using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudioInput : MonoBehaviour, IAudioInputListener
{
    public int channel = 0;
    [SerializeField] private float _width = 0.1f;
    [SerializeField] private float _height = 1f;

    private LineRenderer _lineRenderer;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        AudioInput.Instance.AddChannelListener(channel, this);   
    }


    public void OnAudioBlock(float[] data)
    {
        Debug.Log("CALLED!!!! OnAudioBlock | Length of buffer " + data.Length);
        _lineRenderer.positionCount = data.Length;
        string s = "";
        for (int i = 0; i < data.Length; i++)
        {
            s += data[i] + ", ";
            _lineRenderer.SetPosition(i, new Vector3(i * _width, data[i] * _height, 0));
        }

        Debug.Log(s);
    }

}
