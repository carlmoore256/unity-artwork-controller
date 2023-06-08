using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using CSCore;
using CSCore.SoundIn;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.CoreAudioAPI;
// AudioSettings
using System.Linq;
using System;
public struct AudioDevice {
    string name;
    MMDevice device;
}

public interface IAudioInputListener
{
    void OnAudioBlock(float[] data); 
}

public class AudioInput : MonoBehaviour 
{
    public static AudioInput Instance { get; private set; }
    public MMDevice CurrentDevice { get { return _currentDevice; } }



    private WasapiCapture _capture;
    private MMDevice _currentDevice;

    public string UseDevice = "EVO 4";

    public bool IsCapturing { get; private set; } = false;



    private Dictionary<int, List<IAudioInputListener>> _channelListeners = new Dictionary<int, List<IAudioInputListener>>();


    private void Awake()
    {
        _audioData = new float[_maxChannels][];

        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeAudioDevices();
    }

    private void Update()
    {
        // if audio data has been written to, notify the listeners

        if (_bufferIndex == 0) return;
        
        for (int channel = 0; channel < _maxChannels; channel++)
        {
            if (!_channelListeners.ContainsKey(channel)) continue;

            foreach( var listener in _channelListeners[channel])
            {
                Debug.Log("Notifying listener on channel: " + channel + " buffer index " + _bufferIndex);
                float[] data = new float[_bufferIndex];
                Array.Copy(_audioData[channel], data, _bufferIndex);
                listener?.OnAudioBlock(data);
            }
        }

        ClearBuffers();
    }

    public List<MMDevice> EnumerateDevices()
    {
        var devices = new List<MMDevice>();
        using (var mmdeviceEnumerator = new MMDeviceEnumerator())
        using (var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active))
        {
            foreach (var device in mmdeviceCollection)
            {
                Debug.Log("Found audio input device: " + device.FriendlyName);
                devices.Add(device);
            }
        }
        return devices;
    }

    public void SetAudioDevice(string deviceName)
    {
        var devices = EnumerateDevices();
        var device = devices.FirstOrDefault(d => d.FriendlyName == deviceName);
        if (device != null) SetAudioInputDevice(device);
    }

    public void InitializeAudioDevices()
    {
        var devices = EnumerateDevices();

        if (devices.Count > 0)
        {
            foreach(var device in devices)
            {
                if (device.FriendlyName.ToLower().Contains(UseDevice.ToLower()))
                {
                    SetAudioInputDevice(device);
                    break;
                }
            }
        }
        else
        {
            Debug.Log($"No audio input devices found containing the string: {UseDevice}.");
        }
    }

    private void SetAudioInputDevice(MMDevice device)
    {
        _currentDevice = device;

        if (_capture != null)
        {
            _capture.Stop();
            _capture.Dispose();
            _capture = null;
        }

        _capture = new WasapiCapture() { Device = _currentDevice };
        _capture.Initialize();

        _capture.DataAvailable += (s, e) => OnAudioBlock(e);
    }

    private int _bufferIndex = 0;
    private int _bufferSize = 48000;
    private readonly int _maxChannels = 8;
    // private int _samplesWritten = 0;
    private float[][] _audioData;


    private void OnAudioBlock(DataAvailableEventArgs e)
    {
        Debug.Log("Data available: " + e.ByteCount + " Num channels: " + e.Format.Channels + " Sample rate: " + e.Format.SampleRate + " offset: " + e.Offset + " byte count: " + e.ByteCount);

        int bytesPerSample = e.Format.BytesPerSample;
        int channels = e.Format.Channels;
        int sampleCount = e.ByteCount / bytesPerSample;

        // float[][] audioData = new float[channels][];
        for (int i = 0; i < channels; i++)
        {
            // if (_audioData[i] == null || _audioData[i].Length != sampleCount / channels)
            if (_audioData[i] == null)
                _audioData[i] = new float[_bufferSize];
        }

        for (int i = 0; i < sampleCount; i++)
        {
            int channel = i % channels;
            int index = (i / channels) + _bufferIndex;

            if (index >= _bufferSize)
            {
                Debug.Log("[!] Buffer Overrun, Resetting buffer at index: " + index + " buffer size: " + _bufferSize);
                index = 0;
                // clear all buffers
                ClearBuffers();
            }

            // Adjusted index calculation for interleaved audio
            int adjustedIndex = (i / channels) * bytesPerSample * channels + channel * bytesPerSample;

            // Debug.Log("INDEX : " + index + " ADJUSTED: " + adjustedIndex);

            switch (bytesPerSample)
            {
                case 1:  // 8-bit PCM
                    _audioData[channel][index] = (float)e.Data[adjustedIndex] / byte.MaxValue;
                    break;
                case 2:  // 16-bit PCM
                    _audioData[channel][index] = (float)BitConverter.ToInt16(e.Data, adjustedIndex) / short.MaxValue;
                    break;
                case 3:  // 24-bit PCM
                    _audioData[channel][index] = (float)BitConverter.ToInt32(new byte[] { e.Data[adjustedIndex], e.Data[adjustedIndex + 1], e.Data[adjustedIndex + 2], 0 }, 0) / int.MaxValue;
                    break;
                case 4:  // 32-bit PCM
                    _audioData[channel][index] = (float)BitConverter.ToInt32(e.Data, adjustedIndex) / int.MaxValue;
                    break;
                default:
                    throw new NotSupportedException("Unsupported sample size: " + bytesPerSample);
            }
        }

        _bufferIndex += sampleCount / channels;
    }


    private void ClearBuffers()
    {
        for (int i = 0; i < _maxChannels; i++)
        {
            if (_audioData[i] == null) continue;
            Array.Clear(_audioData[i], 0, _audioData[i].Length);
        }
        _bufferIndex = 0;
    }


    public void AddChannelListener(int channel, IAudioInputListener listener)
    {
        // if (_channelListeners == null) _channelListeners = new Dictionary<int, IAudioInputListener>();
        if (!_channelListeners.ContainsKey(channel)) {
            _channelListeners.Add(channel, new List<IAudioInputListener>());
            _channelListeners[channel].Add(listener);
        }
    }

    public void RemoveChannelListener(int channel)
    {
        if (_channelListeners != null && _channelListeners.ContainsKey(channel)) _channelListeners.Remove(channel);
    }

    // public void RemoveChannelListener(IAudioInputListener listener)
    // {
    //     if (_channelListeners != null && _channelListeners.ContainsValue(listener)) 
    //     {
    //          _channelListeners.Remove(_channelListeners.FirstOrDefault(x => x.Value == listener).Key);
    //     }
    // }

    public void StartCapture()
    {
        if (_capture != null)
        {
            _capture.Start();
            IsCapturing = true;
        } else {
            Debug.Log("No audio input device set.");
        }
    }

    public void StopCapture()
    {
        Debug.Log("Stopping audio capture.");
        if (_capture != null)
        {
            _capture.Stop();
            IsCapturing = false;
        }
    }

    private void OnDestroy()
    {
        _capture.Stop();
        _capture.Dispose();
    }
}

