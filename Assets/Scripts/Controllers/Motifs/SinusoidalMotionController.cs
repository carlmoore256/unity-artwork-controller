using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OscJack;
using UnityEngine;

// should have a state of the controller that can be altered
[System.Serializable]
public class MotifMotionControllerParameters : EndpointParameters
{
    // public override string Address { get; protected set; }

    [ParameterDefinition("Enable Sinusoidal Motion")]
    public bool enableSinusoidalMotion = true;

    [ParameterDefinition("Amplitude (X)")]
    public float moveXAmp = 0.1f;

    [ParameterDefinition("Amplitude (Y)")]
    public float moveYAmp = 0.1f;

    [ParameterDefinition("Amplitude (Z)")]
    public float moveZAmp = 0.1f;

    [ParameterDefinition("Frequency (X)")]
    public float moveXFreq = 0.1f;

    [ParameterDefinition("Frequency (Y)")]
    public float moveYFreq = 0.1f;

    [ParameterDefinition("Frequency (Z)")]
    public float moveZFreq = 0.1f;

    [ParameterDefinition("Phase (X)")]
    public float moveXPhase = 0f;

    [ParameterDefinition("Phase (Y)")]
    public float moveYPhase = 0f;

    [ParameterDefinition("Phase (Z)")]
    public float moveZPhase = 0f;

    // [ParameterDefinition("Sinusoidal speed")]
    // public float sinusoidalSpeed;

    [ParameterDefinition("Enable random motion")]
    public bool enableRandomMotion;

    [ParameterDefinition("Random motion probability")]
    public float randProb;

    [ParameterDefinition("Random motion distance")]
    public float randDist;

    [ParameterDefinition("Enable look at target")]
    public bool enableLookAtTarget;

    [ParameterDefinition("Look at motion")]
    public float lookAtAmount;

    [ParameterDefinition("Lerp to center")]
    public float lerpCenter;

    [ParameterDefinition("Motion Blend Percent")]
    public float blendPercent;

    public string[] commands;
}

// some concrete class should take an instance of an artwork and an artwork behavior


// this is also like a moveable iterator
[RequireComponent(typeof(Artwork))]
public class SinusoidalMotionController : MonoBehaviour, INetworkEndpoint
{
    public int _artworkId = 0;
    public int ArtworkId => _artworkId;
    public string Address => "/motion";
    public Artwork Artwork { get; private set; }
    public MotifMotionControllerParameters parameters;

    private readonly float _minControlValue = 0.01f;
    private Transform _lookAtTarget;
    private Vector3 _currentPhase = Vector3.zero;
    private float _currentBlend = 0f;

    // private float _phase = 0f;

    private Dictionary<Moveable, TransformSnapshot> _targetSnapshots =
        new Dictionary<Moveable, TransformSnapshot>();

    public void ApplyParameters(MotifMotionControllerParameters parameters)
    {
        if (
            parameters.moveXAmp > _minControlValue
            || parameters.moveYAmp > _minControlValue
            || parameters.moveZAmp > _minControlValue
        )
        {
            parameters.enableSinusoidalMotion = true;
        }
        else
        {
            parameters.enableSinusoidalMotion = false;
        }

        if (this.parameters.lerpCenter != parameters.lerpCenter)
        {
            LerpCenter(this.parameters.lerpCenter);
        }

        if (this.parameters.blendPercent != parameters.blendPercent)
        {
            LerpReset(this.parameters.blendPercent);
        }

        this.parameters = parameters;
    }

    void Awake()
    {
        Artwork = GetComponent<Artwork>();
        parameters = new MotifMotionControllerParameters();
    }

    void Start()
    {
        _lookAtTarget = Camera.main.transform;

        Artwork.ForeachMoveable(
            (moveable) =>
            {
                _targetSnapshots.Add(moveable, moveable.CurrentSnapshot);
            }
        );
    }

    private void RegisterCommand(string command)
    {
        switch (command)
        {
            case "reset":
            {
                Artwork.ForeachMoveable(
                    (moveable) =>
                    {
                        moveable.ResetToDefault();
                    }
                );
                break;
            }
        }
    }

    public void LerpReset(float value)
    {
        Artwork.ForeachMoveable(
            (moveable) =>
            {
                moveable.LerpToDefault(value);
            }
        );
    }

    public void LerpCenter(float value)
    {
        var allMoveables = Artwork.AllMoveables;
        var center = allMoveables
            .Select(m => m.CurrentSnapshot.Position)
            .Aggregate((a, b) => a + b);
        center /= allMoveables.Count();
        value = Mathf.Clamp(value, 0f, 1f);
        foreach (var moveable in allMoveables)
        {
            var newPos = Vector3.Lerp(moveable.CurrentSnapshot.Position, center, value);
            _targetSnapshots[moveable].Position = newPos;
        }
    }

    void SinusoidalMotion(Moveable moveable, float normIndex)
    {
        var updatePos = new Vector3(
            Mathf.Sin((_currentPhase.x + normIndex + parameters.moveXPhase) * 2 * Mathf.PI)
                * parameters.moveXAmp,
            Mathf.Cos((_currentPhase.y + normIndex + parameters.moveYPhase) * 2 * Mathf.PI)
                * parameters.moveYAmp,
            Mathf.Sin((_currentPhase.z + normIndex + parameters.moveZPhase) * 2 * Mathf.PI)
                * parameters.moveZAmp
        );

        if (_currentBlend != parameters.blendPercent)
        {
            moveable.BlendPercent = parameters.blendPercent;
        }

        moveable.AddPosition(updatePos);
    }

    void RandomMotion(Moveable moveable, float normIndex)
    {
        if (Random.Range(0f, 1f) > parameters.randProb)
            return;
        var updatePos = new Vector3(
            Random.Range(-1f, 1f) * parameters.randDist,
            Random.Range(-1f, 1f) * parameters.randDist,
            Random.Range(-1f, 1f) * parameters.randDist
        );
        moveable.AddPosition(updatePos);
    }

    // eventually change this so that instead, we just add to the moveable's
    // target snapshot
    void LookAtMotion(Moveable moveable, float normIndex)
    {
        var updateRot = _targetSnapshots[moveable].Rotation;
        var lookAtTarget = _lookAtTarget.position;
        var lookAtPos = moveable.CurrentSnapshot.Position;
        var lookAtDir = lookAtTarget - lookAtPos;
        var lookAtRot = Quaternion.LookRotation(lookAtDir);
        updateRot = Quaternion.Lerp(updateRot, lookAtRot, parameters.lookAtAmount);
        _targetSnapshots[moveable].Rotation = updateRot;
    }

    private void UpdatePhase()
    {
        _currentPhase.x += Time.deltaTime * parameters.moveXFreq;
        _currentPhase.y += Time.deltaTime * parameters.moveYFreq;
        _currentPhase.z += Time.deltaTime * parameters.moveZFreq;
        if (_currentPhase.x > 1)
            _currentPhase.x = 0;
        if (_currentPhase.y > 1)
            _currentPhase.y = 0;
        if (_currentPhase.z > 1)
            _currentPhase.z = 0;
    }

    void Update()
    {
        if (parameters.enableSinusoidalMotion)
        {
            Artwork.ForeachMoveable(SinusoidalMotion);
            UpdatePhase();
        }

        if (parameters.enableRandomMotion)
            Artwork.ForeachMoveable(RandomMotion);
    }
}
