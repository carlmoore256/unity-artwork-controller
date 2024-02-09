using System;
using UnityEngine;

public interface IPhysicsController
{
    public Rigidbody Rigidbody { get; }
    public void SetEnabled(bool enabled);
    public void AddForce(Vector3 force);
}
