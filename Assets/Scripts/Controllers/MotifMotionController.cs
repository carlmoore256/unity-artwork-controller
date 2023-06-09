using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class MotifMotionController : MonoBehaviour, IOscControllable
{
    public int _artworkId = 0;
    public int ArtworkId => _artworkId;
    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/motion";

    private List<Moveable> _moveables = new List<Moveable>();
    private Dictionary<Moveable, TransformSnapshot> _targetSnapshots = new Dictionary<Moveable, TransformSnapshot>();

    [SerializeField] public float _moveXAmount = 0.1f;
    [SerializeField] public float _moveYAmount = 0.1f;
    [SerializeField] public float _moveZAmount = 0.1f;

    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _enableMotion = true;


    Transform LookAtTarget;



    void Start()
    {
        LookAtTarget = GameObject.Find("Main Camera").transform;
        // _moveables.AddRange(FindObjectsOfType<Moveable>());
        _moveables.AddRange(GetComponentsInChildren<Moveable>());

        foreach(var moveable in _moveables)
        {
            if (!_enableMotion) {
                moveable.enabled = false;
            }
            _targetSnapshots.Add(moveable, moveable.CurrentSnapshot);

            // moveable.LookAtTarget = Camera.main.transform;
            
        }

    }

    void OnEnable()
    {
        RegisterEndpoints();
    }

    void OnDisable()
    {
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/sinX");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/cosY");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/sinZ");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/speed");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/range");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lookAtCamera");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/reset");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpCenter");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpReset");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpTarget");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableMotion");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableLookAtTarget");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableLookAtCamera");
        
    }

    // idea : add a camera controller

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{OscAddress}/sinX", (OscDataHandle dataHandle) => {
            _moveXAmount = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/cosY", (OscDataHandle dataHandle) => {
            _moveYAmount = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/sinZ", (OscDataHandle dataHandle) => {
            _moveZAmount = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/speed", (OscDataHandle dataHandle) => {
            _speed = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/range", (OscDataHandle dataHandle) => {
            _speed = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lookAtCamera", (OscDataHandle dataHandle) => {
            foreach(var m in _moveables)
            {
                m.LookAtTarget = Camera.main.transform;
                m.EnableLookAtTarget = !m.EnableLookAtTarget;
            }
        });


        OscManager.Instance.AddEndpoint($"{OscAddress}/reset", (OscDataHandle dataHandle) => {
            foreach(var moveable in _moveables)
            {
                moveable.ResetToDefault();
            }
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lerpCenter", (OscDataHandle dataHandle) => {
            var center = _moveables.Select(m => m.CurrentSnapshot.Position).Aggregate((a, b) => a + b) / _moveables.Count;
            float value = dataHandle.GetElementAsFloat(0);
            foreach(var moveable in _moveables)
            {
                var newPos = Vector3.Lerp(_targetSnapshots[moveable].Position, center, value);
                _targetSnapshots[moveable].Position = newPos;
            }
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lerpReset", (OscDataHandle dataHandle) => {
            foreach(var moveable in _moveables)
            {
                moveable.LerpToDefault(dataHandle.GetElementAsFloat(0));
            }
        });
    }

    float _phase = 0f;

    void OscillatingMovement()
    {
        _phase += Time.deltaTime * _speed;
        if (_phase > 1)
        {
            _phase = 0;
        }

        for(int i = 0; i < _moveables.Count; i++)
        {
            float fracIndex = (float)i / (float)_moveables.Count;
            Moveable moveable = _moveables[i];
            // TransformSnapshot snapshot = moveable.CurrentSnapshot;
            // TransformSnapshot snapshot = _targetSnapshots[moveable];
            Vector3 newPos = _targetSnapshots[moveable].Position;

            // currentPos.x += Random.RandomRange(-0.1f, 0.1f) * moveXAmount;
            newPos.x += Mathf.Sin((_phase + fracIndex) * 2 * Mathf.PI) * _moveXAmount;
            newPos.y += Mathf.Cos((_phase + fracIndex) * 2 * Mathf.PI) * _moveYAmount;
            newPos.z += Mathf.Sin((_phase + fracIndex) * 2 * Mathf.PI) * _moveZAmount;

            // moveable.MoveTo(newPos, 0.6f);
            _targetSnapshots[moveable].Position = newPos;
            // _targetSnapshots[moveable] = snapshot;
        }
    }

    void UpdateTargetSnapshots()
    {
        foreach(var moveable in _moveables)
        {
            _targetSnapshots[moveable] = moveable.CurrentSnapshot;
        }
    }

    void ApplyTargetSnapshots()
    {
        foreach(var moveable in _moveables)
        {
            moveable.TransformTo(_targetSnapshots[moveable]);
        }
    }

    void Update()
    {
        if (!_enableMotion) return;
        UpdateTargetSnapshots();
        OscillatingMovement();
        ApplyTargetSnapshots();
    }
}
