using UnityEngine;
using OscJack;

public class PostProcessingController : MonoBehaviour, IOscControllable
{
    public string OscAddress => "/camera";

    private float _minMIDIValue = 0.01f;



    private void Start()
    {
    }

    private void OnEnable()
    {
        RegisterEndpoints();
    }

    private void OnDisable()
    {
        UnregisterEndpoints();
    }

    private void OnDestroy()
    {
        UnregisterEndpoints();
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

        OscManager.Instance.AddEndpoint($"{OscAddress}/kaliedoscope/reflections", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            // var value = Mathf.Clamp(dataHandle.GetElementAsInt(0), 4, 20);
            var value = dataHandle.GetElementAsFloat(0);
            // normalize to to new range
            value *= 20-4;
            value += 4;
            kaleidoscope.reflections = (int)value;
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/kaliedoscope/stretch", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            var value = Mathf.Clamp(dataHandle.GetElementAsFloat(0), -20f, 20f);
            kaleidoscope.stretch = value;
        });

        // rotation would be cool
        // OscManager.Instance.AddEndpoint($"{OscAddress}/kaliedoscope/rotation", (OscDataHandle dataHandle) => {
        //     var kaleidoscope = GetComponent<KaleidoscopeEffect>();
        //     kaleidoscope.rotation = dataHandle.GetElementAsFloat(0);
        // });

        OscManager.Instance.AddEndpoint(OscAddress + "/echo/intensity", (OscDataHandle dataHandle) => {
            var echo = GetComponent<EchoEffect>();
            SetIntensity(echo, dataHandle.GetElementAsFloat(0));
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/echo/feedback", (OscDataHandle dataHandle) => {
            var echo = GetComponent<EchoEffect>();
            var value = Mathf.Clamp01(dataHandle.GetElementAsFloat(0));
            echo.echoIntensity = value;
        });

        /// GOOD IDEA - make a fade to black screen so that if everything goes wrong, we can easily fade to black,
        //// then reset scene 
        // OscManager.Instance.AddEndpoint($"{OscAddress}/fadeIn", (OscDataHandle dataHandle) => {
        //     FadeInEffect();
        // });
    }

    public void UnregisterEndpoints()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveEndpoint(OscAddress + "/kaleidoscope/toggle");
        OscManager.Instance.RemoveEndpoint(OscAddress + "/echo/toggle");
        OscManager.Instance.RemoveEndpoint(OscAddress + "/kaliedoscope/intensity");
        OscManager.Instance.RemoveEndpoint(OscAddress + "/echo/intensity");
        OscManager.Instance.RemoveEndpoint(OscAddress + "/echo/feedback");
    }

    private void SetIntensity(ICameraEffect effect, float value)
    {
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