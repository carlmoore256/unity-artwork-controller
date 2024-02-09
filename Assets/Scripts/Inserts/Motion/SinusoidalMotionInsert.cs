using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OscJack;
using UnityEngine;

// public abstract class ControllerParameters
// {

// }

// public interface IController
// {
//     ControllerParameters GetParameters();
//     void SetParameters(ControllerParameters parameters);
// }


// should have a state of the controller that can be altered
// some concrete class should take an instance of an artwork and an artwork behavior

// [CreateAssetMenu(
//     fileName = "SinusoidalMotionInsertParameters",
//     menuName = "Inserts/SinusoidalMotionParameters"
// )]
public class SinusoidalMotionInsertParameters : InsertParameters
{
    public ParameterValue<bool> enable = new(true);
    public RangedParameterValue moveXAmp = new(0.1f, 0f, 10f, "Amplitude (X)");
    public RangedParameterValue moveYAmp = new(0.1f, 0f, 10f, "Amplitude (Y)");
    public RangedParameterValue moveZAmp = new(0.1f, 0f, 10f, "Amplitude (Z)");
    public RangedParameterValue moveXFreq = new(1f, 0f, 5f, "Frequency (X)");
    public RangedParameterValue moveYFreq = new(1f, 0f, 2f, "Frequency (Y)");
    public RangedParameterValue moveZFreq = new(1f, 0f, 2f, "Frequency (Z)");
    public RangedParameterValue moveXPhase = new(0f, 0f, 1f, "Phase (X)");
    public RangedParameterValue moveYPhase = new(0f, 0f, 1f, "Phase (Y)");
    public RangedParameterValue moveZPhase = new(0f, 0f, 1f, "Phase (Z)");
    public ParameterValue<bool> enableRandomMotion = new(false);
    public RangedParameterValue randProb = new(0.1f, 0f, 1f);
    public RangedParameterValue randDist = new(0.1f, 0f, 10f);
    public ParameterValue<bool> enableLookAtTarget = new(false);
    public RangedParameterValue lookAtAmount = new(0.1f, 0f, 1f);
    public RangedParameterValue lerpCenter = new(0f, 0f, 1f);
    public RangedParameterValue blendPercent = new(0f, 0f, 1f);
    public ParameterValue<float> fooBar = new(0.5f);
}


public class SinusoidalMotionInsert : MonoBehaviourWithId, INetworkEndpoint, IInsert
{
    public string Name => "Sinusoidal Motion";

    public int _artworkId = 0;
    public int ArtworkId => _artworkId;
    public string Address => "/motion";
    public IMovableIterator _movableIterator { get; private set; }

    private SinusoidalMotionInsertParameters _parameters = new SinusoidalMotionInsertParameters();

    private readonly float _minControlValue = 0.01f;
    private Transform _lookAtTarget;
    private Vector3 _currentPhase = Vector3.zero;
    private float _currentBlend = 0f;

    private Dictionary<Moveable, TransformSnapshot> _targetSnapshots =
        new Dictionary<Moveable, TransformSnapshot>();

    public InsertParameters GetParameters()
    {
        return _parameters;
    }

    // public void UpdateParameters(string json)
    // {
    //     var items = JsonUtility.FromJson<IEnumerable<object>>(json);
    //     // TODO - make it do something. Ultimately, we wanted to be able to update parameters OUTSIDE of this class
    //     // _parameters = JsonUtility.FromJson<SinusoidalMotionInsertParameters>(json);
    // }

    // public void UpdateParameters(
    //     InsertParametersPatcher<SinusoidalMotionInsertParameters> parameters
    // )
    // {
    //     _parameters = parameters;
    // }

    private void OnDisable()
    {
        Unregister();
    }

    public void Register(string baseAddress)
    {
        Debug.Log($"{baseAddress}/motion/sinX");

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/sinX",
            (OscDataHandle dataHandle) =>
            {
                _parameters.moveXAmp.SetValue(dataHandle.GetElementAsFloat(0));
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/cosY",
            (OscDataHandle dataHandle) =>
            {
                _parameters.moveYAmp.SetValue(dataHandle.GetElementAsFloat(0));
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/sinZ",
            (OscDataHandle dataHandle) =>
            {
                _parameters.moveZAmp.SetValue(dataHandle.GetElementAsFloat(0));
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/speed",
            (OscDataHandle dataHandle) =>
            {
                var speed = dataHandle.GetElementAsFloat(0);
                _parameters.moveXFreq.SetValue(speed);
                _parameters.moveYFreq.SetValue(speed);
                _parameters.moveZFreq.SetValue(speed);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/randProb",
            (OscDataHandle dataHandle) =>
            {
                _parameters.randProb.SetValue(Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f));
                if (_parameters.randProb < _minControlValue)
                    _parameters.enableRandomMotion.SetValue(false);
                else
                    _parameters.enableRandomMotion.SetValue(true);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/randDist",
            (OscDataHandle dataHandle) =>
            {
                _parameters.randDist.SetValue(dataHandle.GetElementAsFloat(0));
                if (_parameters.randDist < _minControlValue)
                    _parameters.enableRandomMotion.SetValue(false);
                else
                    _parameters.enableRandomMotion.SetValue(true);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/lookAt",
            (OscDataHandle dataHandle) =>
            {
                _parameters.lookAtAmount.SetValue(
                    Mathf.Clamp(dataHandle.GetElementAsFloat(0), 0f, 1f)
                );
                if (_parameters.lookAtAmount < _minControlValue)
                    _parameters.enableLookAtTarget.SetValue(false);
                else
                    _parameters.enableLookAtTarget.SetValue(true);
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/reset",
            (OscDataHandle dataHandle) =>
            {
                _movableIterator.ForeachMoveable(
                    (moveable) =>
                    {
                        moveable.ResetToDefault();
                    }
                );
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/lerpCenter",
            (OscDataHandle dataHandle) =>
            {
                LerpCenter(dataHandle.GetElementAsFloat(0));
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/motion/lerpReset",
            (OscDataHandle dataHandle) =>
            {
                LerpReset(dataHandle.GetElementAsFloat(0));
            },
            this
        );
    }

    private Vector3 MoveablesCenter()
    {
        var center = new Vector3(0, 0, 0);
        _movableIterator.ForeachMoveable(
            (moveable) =>
            {
                center += moveable.CurrentSnapshot.Position;
            }
        );
        center /= _movableIterator.NumMoveables;
        return center;
    }

    public void Unregister()
    {
        if (OscManager.Instance == null)
            return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
    }

    void Awake()
    {
        _movableIterator = GetComponent<SegmentedPaintingArtwork>();
        // Parameters = new MotifMotionControllerParameters();
    }

    void Start()
    {
        _lookAtTarget = Camera.main.transform;

        _movableIterator.ForeachMoveable(
            (moveable) =>
            {
                _targetSnapshots.Add(moveable, moveable.CurrentSnapshot);
            }
        );
    }

    public void LerpReset(float value)
    {
        _movableIterator.ForeachMoveable(
            (moveable) =>
            {
                moveable.LerpToDefault(value);
            }
        );
    }

    public void LerpCenter(float value)
    {
        var center = MoveablesCenter();
        value = Mathf.Clamp(value, 0f, 1f);
        _movableIterator.ForeachMoveable(
            (moveable) =>
            {
                var newPos = Vector3.Lerp(moveable.CurrentSnapshot.Position, center, value);
                moveable.AddPosition(newPos);
                // _targetSnapshots[moveable].Position = newPos;
            }
        );
        // var allMoveables = _movableIterator.AllMoveables;
        // var center = allMoveables
        //     .Select(m => m.CurrentSnapshot.Position)
        //     .Aggregate((a, b) => a + b);
        // center /= allMoveables.Count();
        // value = Mathf.Clamp(value, 0f, 1f);
        // foreach (var moveable in allMoveables)
        // {
        //     var newPos = Vector3.Lerp(moveable.CurrentSnapshot.Position, center, value);
        //     _targetSnapshots[moveable].Position = newPos;
        // }
    }

    void SinusoidalMotion(Moveable moveable, float normIndex)
    {
        var updatePos = new Vector3(
            Mathf.Sin((_currentPhase.x + normIndex + _parameters.moveXPhase) * 2 * Mathf.PI)
                * _parameters.moveXAmp,
            Mathf.Cos((_currentPhase.y + normIndex + _parameters.moveYPhase) * 2 * Mathf.PI)
                * _parameters.moveYAmp,
            Mathf.Sin((_currentPhase.z + normIndex + _parameters.moveZPhase) * 2 * Mathf.PI)
                * _parameters.moveZAmp
        );

        if (_currentBlend != _parameters.blendPercent)
        {
            moveable.BlendPercent = _parameters.blendPercent;
        }

        moveable.AddPosition(updatePos);
    }

    // void RandomMotion(Moveable moveable, float normIndex)
    // {
    //     if (Random.Range(0f, 1f) > _parameters.randProb)
    //         return;
    //     var updatePos = new Vector3(
    //         Random.Range(-1f, 1f) * _parameters.randDist,
    //         Random.Range(-1f, 1f) * _parameters.randDist,
    //         Random.Range(-1f, 1f) * _parameters.randDist
    //     );
    //     moveable.AddPosition(updatePos);
    // }

    // eventually change this so that instead, we just add to the moveable's
    // target snapshot
    // void LookAtMotion(Moveable moveable, float normIndex)
    // {
    //     var updateRot = _targetSnapshots[moveable].Rotation;
    //     var lookAtTarget = _lookAtTarget.position;
    //     var lookAtPos = moveable.CurrentSnapshot.Position;
    //     var lookAtDir = lookAtTarget - lookAtPos;
    //     var lookAtRot = Quaternion.LookRotation(lookAtDir);
    //     updateRot = Quaternion.Lerp(updateRot, lookAtRot, _parameters.lookAtAmount);
    //     _targetSnapshots[moveable].Rotation = updateRot;
    // }

    private void UpdatePhase()
    {
        _currentPhase.x += Time.deltaTime * _parameters.moveXFreq;
        _currentPhase.y += Time.deltaTime * _parameters.moveYFreq;
        _currentPhase.z += Time.deltaTime * _parameters.moveZFreq;
        if (_currentPhase.x > 1)
            _currentPhase.x = 0;
        if (_currentPhase.y > 1)
            _currentPhase.y = 0;
        if (_currentPhase.z > 1)
            _currentPhase.z = 0;
    }

    void Update()
    {
        if (_parameters.enable)
        {
            _movableIterator.ForeachMoveable(SinusoidalMotion);
            UpdatePhase();
        }

        // if (Parameters.Values.enableRandomMotion)
        //     Artwork.ForeachMoveable(RandomMotion);
    }
}
