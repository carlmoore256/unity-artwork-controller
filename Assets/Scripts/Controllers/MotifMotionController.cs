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
    public bool enableSinusoidalMotion;

    [ParameterDefinition("X Movement")]
    public float moveXAmount;

    [ParameterDefinition("Y Movement")]
    public float moveYAmount;

    [ParameterDefinition("Z Movement")]
    public float moveZAmount;

    [ParameterDefinition("Sinusoidal speed")]
    public float sinusoidalSpeed;

    [ParameterDefinition("Random motion range")]
    public float range;

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

    [ParameterDefinition("Lerp to default")]
    public float lerpReset;

    public string[] commands;

}

[RequireComponent(typeof(Artwork))]
public class MotifMotionController
    : MonoBehaviour,
        INetworkEndpoint
        // INetworkEndpoint<MotifMotionControllerParameters>
{
    public int _artworkId = 0;
    public int ArtworkId => _artworkId;
    public string Address => "/motion";
    public Artwork Artwork { get; private set; }
    public MotifMotionControllerParameters Parameters { get; private set; }

    private readonly float _minControlValue = 0.01f;
    private Transform _lookAtTarget;
    private float _phase = 0f;

    private Dictionary<Moveable, TransformSnapshot> _targetSnapshots =
        new Dictionary<Moveable, TransformSnapshot>();

    public void ApplyParameters(MotifMotionControllerParameters parameters)
    {
        if (
            parameters.moveXAmount > _minControlValue
            || parameters.moveYAmount > _minControlValue
            || parameters.moveZAmount > _minControlValue
        )
        {
            parameters.enableSinusoidalMotion = true;
        }
        else
        {
            parameters.enableSinusoidalMotion = false;
        }

        if (Parameters.lerpCenter != parameters.lerpCenter)
        {
            LerpCenter(Parameters.lerpCenter);
        }

        if (Parameters.lerpReset != parameters.lerpReset)
        {
            LerpReset(Parameters.lerpReset);
        }

        Parameters = parameters;
    }

    void Awake()
    {
        Artwork = GetComponent<Artwork>();
        Parameters = new MotifMotionControllerParameters();
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
        var updatePos = _targetSnapshots[moveable].Position;
        updatePos.x += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * Parameters.moveXAmount;
        updatePos.y += Mathf.Cos((_phase + normIndex) * 2 * Mathf.PI) * Parameters.moveYAmount;
        updatePos.z += Mathf.Sin((_phase + normIndex) * 2 * Mathf.PI) * Parameters.moveZAmount;
        _targetSnapshots[moveable].Position = updatePos;
    }

    void RandomMotion(Moveable moveable, float normIndex)
    {
        if (Random.Range(0f, 1f) > Parameters.randProb)
            return;
        var updatePos = _targetSnapshots[moveable].Position;
        updatePos.y += Random.Range(-1f, 1f) * Parameters.randDist;
        updatePos.x += Random.Range(-1f, 1f) * Parameters.randDist;
        updatePos.z += Random.Range(-1f, 1f) * Parameters.randDist;
        _targetSnapshots[moveable].Position = updatePos;
    }

    void LookAtMotion(Moveable moveable, float normIndex)
    {
        var updateRot = _targetSnapshots[moveable].Rotation;
        var lookAtTarget = _lookAtTarget.position;
        var lookAtPos = moveable.CurrentSnapshot.Position;
        var lookAtDir = lookAtTarget - lookAtPos;
        var lookAtRot = Quaternion.LookRotation(lookAtDir);
        updateRot = Quaternion.Lerp(updateRot, lookAtRot, Parameters.lookAtAmount);
        _targetSnapshots[moveable].Rotation = updateRot;
    }

    private void UpdatePhase()
    {
        _phase += Time.deltaTime * Parameters.sinusoidalSpeed;
        if (_phase > 1)
            _phase = 0;
    }

    // we have to do this because there may be multiple operations on all of the moveables
    private void UpdateTargetSnapshots() =>
        Artwork.ForeachMoveable(movable => _targetSnapshots[movable] = movable.CurrentSnapshot);

    private void ApplyTargetSnapshots() =>
        Artwork.ForeachMoveable(movable => movable.TransformTo(_targetSnapshots[movable]));

    void Update()
    {
        UpdateTargetSnapshots();
        if (Parameters.enableSinusoidalMotion)
        {
            Artwork.ForeachMoveable(SinusoidalMotion);
            UpdatePhase();
        }
        if (Parameters.enableRandomMotion)
            Artwork.ForeachMoveable(RandomMotion);
        if (Parameters.enableLookAtTarget)
            Artwork.ForeachMoveable(LookAtMotion);
        ApplyTargetSnapshots();
    }
}
