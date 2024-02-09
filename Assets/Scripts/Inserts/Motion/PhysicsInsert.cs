using System.Collections.Generic;
using UnityEngine;

public class PhysicsInsertParameters : InsertParameters
{
    public ParameterValue<bool> enable = new(false);
    public ParameterValue<bool> useGravity = new(false);
    public RangedParameterValue mass = RangedParameterValue.NormalizedRange("Mass");
    public RangedParameterValue drag = RangedParameterValue.NormalizedRange("Drag");
    public RangedParameterValue angularDrag = RangedParameterValue.NormalizedRange("Angular Drag");
    public RangedParameterValue gravityScale = RangedParameterValue.NormalizedRange(
        "Gravity Scale"
    );

    public RangedParameterValue maxVelocity = RangedParameterValue.NormalizedRange("Max Speed");
    public RangedParameterValue maxAngularVelocity = new(7f, 0f, 100f, "Max Angular Velocity");

    public RangedParameterValue explosionForce = RangedParameterValue.NormalizedRange(
        "Explosion Force"
    );

    public TriggerParameterValue explode = new("Explode");
}

public class PhysicsInsert : MonoBehaviourWithId, IInsert
{
    public string Name => "Sprite Physics";

    private PhysicsInsertParameters _parameters = new PhysicsInsertParameters();

    public InsertParameters GetParameters()
    {
        return _parameters;
    }

    private IPhysicsController[] _physicsControllers;

    private void OnEnable()
    {
        // make this getComponents process simple, rather than requiring an IArtwork
        // simply get any components in children that have an IPhysicsController
        _physicsControllers = GetComponentsInChildren<IPhysicsController>();
        // _parameters.explode.OnTrigger += Explode;
        _parameters.OnParameterChanged += OnParameterChanged;
        _parameters.enable.OnValueChanged += OnEnableChanged;
    }

    private void OnDisable()
    {
        // _parameters.explode.OnTrigger -= Explode;
        _parameters.OnParameterChanged -= OnParameterChanged;
        _parameters.enable.OnValueChanged -= OnEnableChanged;
    }

    private void OnEnableChanged(bool enabled)
    {
        foreach (var controller in _physicsControllers)
        {
            controller.SetEnabled(enabled);
        }
    }

    private void OnParameterChanged(BaseParameterValue parameter)
    {
        if (parameter is TriggerParameterValue)
            return;

        // set the parameter value on all physics controllers
        foreach (var controller in _physicsControllers)
        {
            controller.Rigidbody.mass = _parameters.mass;
            controller.Rigidbody.drag = _parameters.drag;
            controller.Rigidbody.angularDrag = _parameters.angularDrag;
            controller.Rigidbody.maxLinearVelocity = _parameters.maxVelocity;
            controller.Rigidbody.maxAngularVelocity = _parameters.maxAngularVelocity;
            controller.Rigidbody.useGravity = _parameters.useGravity;
        }
    }

    private void Explode()
    {
        var centerOfMass = CenterOfMass();
        foreach (var controller in _physicsControllers)
        {
            controller.Rigidbody.AddExplosionForce(
                _parameters.explosionForce * 100f,
                centerOfMass,
                10f
            );
        }
    }

    private Vector3 CenterOfMass()
    {
        var center = Vector3.zero;
        foreach (var controller in _physicsControllers)
        {
            center += controller.Rigidbody.worldCenterOfMass;
        }
        return center / _physicsControllers.Length;
    }
}
