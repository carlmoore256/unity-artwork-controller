using UnityEditor;
using UnityEngine;
using CSCore.CoreAudioAPI;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(AudioInput))]
public class AudioInputEditor : Editor
{
    List<MMDevice> _devices = new List<MMDevice>();
    int _selectedDeviceIndex = 0;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        AudioInput audioInput = (AudioInput)target;

        if (GUILayout.Button("Refresh Devices"))
        {
            _devices = audioInput.EnumerateDevices();
        }

        if (_devices != null && _devices.Count > 0) {

            if (audioInput.CurrentDevice != null)
            {
                EditorGUILayout.LabelField($"Current Device: [{audioInput.CurrentDevice.FriendlyName}] | {audioInput.CurrentDevice.DeviceFormat.Channels} Channels | {audioInput.CurrentDevice.DeviceFormat.SampleRate} Hz");
            }

            ToggleCapture(audioInput);
        }
    }


    public void ToggleCapture(AudioInput audioInput)
    {
        string[] options = _devices.Select(device => device.FriendlyName).ToArray();
        _selectedDeviceIndex = EditorGUILayout.Popup("Device", _selectedDeviceIndex, options);
            
        if (audioInput.IsCapturing)
        {
            if(GUILayout.Button("Stop Capture"))
            {
                Debug.Log("Stopping capture");
                audioInput.StopCapture();
            }
        }
        else
        {
            if (GUILayout.Button("Start Capture"))
            {
                Debug.Log("Starting capture");
                audioInput.SetAudioDevice(options[_selectedDeviceIndex]);
                audioInput.StartCapture();
            }
        }
    }
}
