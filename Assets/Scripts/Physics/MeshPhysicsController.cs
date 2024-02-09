using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MeshPhysicsController : MonoBehaviour, IPhysicsController
{
    public Rigidbody Rigidbody => _rigidbody;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    public void AddForce(Vector3 force)
    {
        _rigidbody.AddForce(force);
    }

    public void SetEnabled(bool usePhysics)
    {
        if (!usePhysics)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        _rigidbody.isKinematic = !usePhysics;
    }
}
