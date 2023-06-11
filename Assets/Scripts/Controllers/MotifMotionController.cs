using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;


// consider adding a MotionPlugin or IMotionPlugin, or even IEffectPlugin, that has 
// access to an artwork, and does things to it. Its behavior can be tied to one of these controllers

public class MotifMotionController : MonoBehaviour, IOscControllable
{
    public int _artworkId = 0;
    public int ArtworkId => _artworkId;
    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/motion";

    // private List<Moveable> _moveables = new List<Moveable>();
    private Dictionary<Moveable, TransformSnapshot> _targetSnapshots = new Dictionary<Moveable, TransformSnapshot>();


    [Header("Sinusoidal Motion")]
    [SerializeField] public float _moveXAmount = 0.1f;
    [SerializeField] public float _moveYAmount = 0.1f;
    [SerializeField] public float _moveZAmount = 0.1f;

    [SerializeField] private float _randomMotionInitRange = 0.1f;

    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _enableSinusoidalMotion = true;
    private float _phase = 0f;
    
    [Header("Random Motion")]
    [SerializeField] private float _randProb = 0.1f;
    [SerializeField] private float _randDist = 0.0f;
    [SerializeField] private bool _enableRandomMotion = true;

    [Header("Look At")]
    [SerializeField] private float _lookAtAmount = 0.0f;
    [SerializeField] private bool _enableLookAtTarget = false;

    

    private readonly float _minControlValue = 0.01f; 

    private Transform _lookAtTarget;


    void Start()
    {
        // _lookAtTarget = GameObject.Find("Main Camera").transform;
        _lookAtTarget = Camera.main.transform;

        Artwork.ForeachMoveable((moveable) => {
            _targetSnapshots.Add(moveable, moveable.CurrentSnapshot);
        });

        if (_randDist == 0.0f)
        {
            _enableRandomMotion = false;
        }
    }

    void OnEnable()
    {
        RegisterEndpoints();


        _moveXAmount = Random.Range(0f, _randomMotionInitRange);
        _moveYAmount = Random.Range(0f, _randomMotionInitRange);
        _moveZAmount = Random.Range(0f, _randomMotionInitRange);
    }

    void OnDisable()
    {
        UnregisterEndpoints();
    }

    void OnDestroy()
    {
        UnregisterEndpoints();
    }

    public void RegisterEndpoints()
    {
        OscManager.Instance.AddEndpoint($"{OscAddress}/sinX", (OscDataHandle dataHandle) => {
            _moveXAmount = dataHandle.GetElementAsFloat(0);
            CheckSinusoidalMotionStatus();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/cosY", (OscDataHandle dataHandle) => {
            _moveYAmount = dataHandle.GetElementAsFloat(0);
            CheckSinusoidalMotionStatus();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/sinZ", (OscDataHandle dataHandle) => {
            _moveZAmount = dataHandle.GetElementAsFloat(0);
            CheckSinusoidalMotionStatus();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/speed", (OscDataHandle dataHandle) => {
            _speed = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/range", (OscDataHandle dataHandle) => {
            _speed = dataHandle.GetElementAsFloat(0);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/randProb", (OscDataHandle dataHandle) => {
            _randProb = Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f);
            if (_randProb < _minControlValue) _enableRandomMotion = false;
            else _enableRandomMotion = true;
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/randDist", (OscDataHandle dataHandle) => {
            _randDist = dataHandle.GetElementAsFloat(0);
            if (_randDist < _minControlValue) _enableRandomMotion = false;
            else _enableRandomMotion = true;
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lookAt", (OscDataHandle dataHandle) => {
            _lookAtAmount = Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f);
            if (_lookAtAmount < _minControlValue) _enableLookAtTarget = false;
            else _enableLookAtTarget = true;
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/reset", (OscDataHandle dataHandle) => {
            Artwork.ForeachMoveable((moveable) => {
                moveable.ResetToDefault();
            });
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lerpCenter", (OscDataHandle dataHandle) => {
            var allMoveables = Artwork.AllMoveables;
            var center = allMoveables.Select(m => m.CurrentSnapshot.Position).Aggregate((a, b) => a + b);
            center /= allMoveables.Count();
            float value = Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f);
            foreach(var moveable in allMoveables)
            {
                var newPos = Vector3.Lerp(moveable.CurrentSnapshot.Position, center, value);
                _targetSnapshots[moveable].Position = newPos;
            }
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/lerpReset", (OscDataHandle dataHandle) => {
            Artwork.ForeachMoveable((moveable) => {
                moveable.LerpToDefault(dataHandle.GetElementAsFloat(0));
            });
        });
    }

    public void UnregisterEndpoints()
    {
        if (OscManager.Instance == null) return;
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/sinX");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/cosY");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/sinZ");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/speed");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/range");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lookAtCamera");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/reset");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpCenter");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpReset");

        OscManager.Instance.RemoveEndpoint($"{OscAddress}/randProbab");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/randDist");

        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/lerpTarget");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableMotion");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableLookAtTarget");
        // OscManager.Instance.RemoveEndpoint($"{OscAddress}/enableLookAtCamera");
    }

    private void CheckSinusoidalMotionStatus()
    {
        if (_moveXAmount > _minControlValue || _moveYAmount > _minControlValue || _moveZAmount > _minControlValue) {
            _enableSinusoidalMotion = true;
        } else {
            _enableSinusoidalMotion = false;
        }
    }


    void SinusoidalMotion()
    {
        _phase += Time.deltaTime * _speed;
        if (_phase > 1)
        {
            _phase = 0;
        }

        Artwork.ForeachMoveable((moveable, normIndex) => {
            var updatePos = _targetSnapshots[moveable].Position;
            updatePos.x += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * _moveXAmount;
            updatePos.y += Mathf.Cos((_phase + normIndex) * 2 * Mathf.PI) * _moveYAmount;
            updatePos.z += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * _moveZAmount;
            _targetSnapshots[moveable].Position = updatePos;
        });
    }

    void RandomMotion()
    {
        // var numToMove = Artwork.AllMoveables.Count() * _randProbability;
        Artwork.ForeachMoveable((moveable, normIndex) => {
            if (Random.Range(0f, 1f) > _randProb)
                return;
            var updatePos = _targetSnapshots[moveable].Position;
            // var moveScalar = Random.Range(_randRangeLo, _randRangeHi);
            updatePos.x += Random.Range(-1f, 1f) * _randDist;
            updatePos.y += Random.Range(-1f, 1f) * _randDist;
            updatePos.z += Random.Range(-1f, 1f) * _randDist;
            _targetSnapshots[moveable].Position = updatePos;
        });
    }

    void LookAtMotion()
    {
        // update each moveable to look at _lookAtTarget
        Artwork.ForeachMoveable((moveable, normIndex) => {
            var updateRot = _targetSnapshots[moveable].Rotation;
            var lookAtTarget = _lookAtTarget.position;
            var lookAtPos = moveable.CurrentSnapshot.Position;
            var lookAtDir = lookAtTarget - lookAtPos;
            var lookAtRot = Quaternion.LookRotation(lookAtDir);
            updateRot = Quaternion.Lerp(updateRot, lookAtRot, _lookAtAmount);
            _targetSnapshots[moveable].Rotation = updateRot;
        });
    }

    // we have to do this because there may be multiple operations on all of the moveables
    void UpdateTargetSnapshots()
    {
        Artwork.ForeachMoveable(movable => _targetSnapshots[movable] = movable.CurrentSnapshot);
    }

    void ApplyTargetSnapshots()
    {
        Artwork.ForeachMoveable(movable => movable.TransformTo(_targetSnapshots[movable]));
    }
    
    void Update()
    {
        UpdateTargetSnapshots();
        if (_enableSinusoidalMotion) SinusoidalMotion();
        if (_enableRandomMotion) RandomMotion();   
        if (_enableLookAtTarget) LookAtMotion();     
        ApplyTargetSnapshots();
    }
}