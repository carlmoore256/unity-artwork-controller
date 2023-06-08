using UnityEngine;
using OscJack;

public class CameraEffectController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/camera";

    private float _minMIDIValue = 0.01f;

    private void Start()
    {
        RegisterEndpoints();
    }

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint(OscAddress + "/kaleidoscope/toggle", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            ToggleICameraEffect(kaleidoscope);
        });

        OscManager.Instance.AddEndpoint(OscAddress + "/echo/toggle", (OscDataHandle dataHandle) => {
            var echo = GetComponent<EchoEffect>();
            ToggleICameraEffect(echo);
        });

        OscManager.Instance.AddEndpoint(OscAddress + "/kaliedoscope/intensity", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            SetIntensity(kaleidoscope, dataHandle.GetElementAsFloat(0));
        });

        OscManager.Instance.AddEndpoint(OscAddress + "/echo/intensity", (OscDataHandle dataHandle) => {
            var echo = GetComponent<EchoEffect>();
            SetIntensity(echo, dataHandle.GetElementAsFloat(0));
        });
    }

    private void SetIntensity(ICameraEffect effect, float value)
    {
        // Debug.Log("TRYING TO DISABLE!");
        // (effect as MonoBehaviour).enabled = false;


        Debug.Log("Setting intensity to " + value);
        if (!(effect as MonoBehaviour).enabled && value > _minMIDIValue) {
            Debug.Log("Enabling effect");
            (effect as MonoBehaviour).enabled = true;
            effect.FadeIn(null, value);

        } else if ((effect as MonoBehaviour).enabled && value > _minMIDIValue) {
            effect.Intensity = value;
        } else if ((effect as MonoBehaviour).enabled && value < _minMIDIValue) {
            Debug.Log("Disabling effect");
            effect.FadeOut(() => {
                // (effect as MonoBehaviour)
                (effect as MonoBehaviour).enabled = false;
            });
        }
    }

    private void ToggleICameraEffect(ICameraEffect effect)
    {
        if (!(effect as MonoBehaviour).enabled) {
            (effect as MonoBehaviour).enabled = true;
            effect.FadeIn(null, 1);
        } else {
            effect.FadeOut(() => {
                (effect as MonoBehaviour).enabled = false;
            });
        }
    }


}