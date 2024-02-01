using UnityEngine;
using OscJack;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour, INetworkEndpoint
{
    public string Address => "/camera";

    private float _minMIDIValue = 0.01f;

    [SerializeField] private PostProcessProfile _postProcessProfile;



    private void Start()
    {
    }

    private void OnEnable()
    {
        Register("/camera");
    }

    private void OnDisable()
    {
        Unregister();
    }

    private void OnDestroy()
    {
        Unregister();
    }
    

    public void Register(string baseAddress)
    {
        OscManager.Instance.AddEndpoint(baseAddress + "/kaleidoscope/toggle", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            ToggleICameraEffect(kaleidoscope);
        }, this);

        OscManager.Instance.AddEndpoint(baseAddress + "/echo/toggle", (OscDataHandle dataHandle) => {
            var echo = GetComponent<EchoEffect>();
            ToggleICameraEffect(echo);
        }, this);

        OscManager.Instance.AddEndpoint(baseAddress + "/kaliedoscope/intensity", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            SetIntensity(kaleidoscope, dataHandle.GetElementAsFloat(0));
        }, this);

        OscManager.Instance.AddEndpoint($"{baseAddress}/kaliedoscope/reflections", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            // var value = Mathf.Clamp(dataHandle.GetElementAsInt(0), 4, 20);
            var value = dataHandle.GetElementAsFloat(0);
            // normalize to to new range
            value *= 20-4;
            value += 4;
            kaleidoscope.reflections = (int)value;
        }, this);

        OscManager.Instance.AddEndpoint($"{baseAddress}/kaliedoscope/stretch", (OscDataHandle dataHandle) => {
            var kaleidoscope = GetComponent<KaleidoscopeEffect>();
            var value = Mathf.Clamp(dataHandle.GetElementAsFloat(0), -20f, 20f);
            kaleidoscope.stretch = value;
        }, this);

        // rotation would be cool
        // OscManager.Instance.AddEndpoint($"{OscAddress}/kaliedoscope/rotation", (OscDataHandle dataHandle) => {
        //     var kaleidoscope = GetComponent<KaleidoscopeEffect>();
        //     kaleidoscope.rotation = dataHandle.GetElementAsFloat(0);
        // });

        OscManager.Instance.AddEndpoint(baseAddress + "/echo/intensity", (OscDataHandle dataHandle) => {
            var echo = GetComponent<EchoEffect>();
            SetIntensity(echo, dataHandle.GetElementAsFloat(0));
        }, this);


        OscManager.Instance.AddEndpoint($"{baseAddress}/echo/feedback", (OscDataHandle dataHandle) => {
            var echo = GetComponent<EchoEffect>();
            var value = Mathf.Clamp01(dataHandle.GetElementAsFloat(0));
            echo.echoIntensity = value;
        }, this);

        OscManager.Instance.AddEndpoint($"{baseAddress}/bloom/intensity", (OscDataHandle dataHandle) => {
            var bloom = _postProcessProfile.GetSetting<Bloom>();
            var value = Mathf.Clamp01(dataHandle.GetElementAsFloat(0));
            if (value < _minMIDIValue) {
                bloom.active = false;
            } else {
                bloom.active = true;
            }
            bloom.intensity.value = value;
        }, this);

        OscManager.Instance.AddEndpoint($"{baseAddress}/bloom/threshold", (OscDataHandle dataHandle) => {
            var bloom = _postProcessProfile.GetSetting<Bloom>();
            var value = Mathf.Clamp01(dataHandle.GetElementAsFloat(0));
            bloom.threshold.value = value;
        }, this);

        OscManager.Instance.AddEndpoint($"{baseAddress}/bloom/softKnee", (OscDataHandle dataHandle) => {
            var bloom = _postProcessProfile.GetSetting<Bloom>();
            var value = Mathf.Clamp01(dataHandle.GetElementAsFloat(0));
            bloom.softKnee.value = value;
        }, this);

        OscManager.Instance.AddEndpoint($"{baseAddress}/abberation/intensity", (OscDataHandle dataHandle) => {
            var abberation = _postProcessProfile.GetSetting<ChromaticAberration>();
            var value = Mathf.Clamp01(dataHandle.GetElementAsFloat(0));
            if (value < _minMIDIValue) {
                abberation.active = false;
            } else {
                abberation.active = true;
            }
            abberation.intensity.value = value;
        }, this);


        /// GOOD IDEA - make a fade to black screen so that if everything goes wrong, we can easily fade to black,
        //// then reset scene 
        // OscManager.Instance.AddEndpoint($"{OscAddress}/fadeIn", (OscDataHandle dataHandle) => {
        //     FadeInEffect();
        // });
    }

    public void Unregister()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
        // OscManager.Instance.RemoveEndpoint(Address + "/kaleidoscope/toggle");
        // OscManager.Instance.RemoveEndpoint(Address + "/echo/toggle");
        // OscManager.Instance.RemoveEndpoint(Address + "/kaliedoscope/intensity");
        // OscManager.Instance.RemoveEndpoint(Address + "/echo/intensity");
        // OscManager.Instance.RemoveEndpoint(Address + "/echo/feedback");
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