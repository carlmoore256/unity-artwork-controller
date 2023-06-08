using UnityEngine;
using System;

public class SpritePhysics : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void OnEnable()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;
        Mesh mesh = SpriteToMesh(sprite);

        if (gameObject.GetComponent<Rigidbody>() == null) {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        } else {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
        }


        if (gameObject.GetComponent<MeshCollider>() == null) {
            gameObject.AddComponent<MeshCollider>();
        }

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshCollider>().convex = true;

        ToggleGravity(false);
        TogglePhysics(false);

        // exclude all layers except layer 6
        _rigidbody.gameObject.layer = 6;
        _rigidbody.excludeLayers = 1 << 6;
        // _rigidbody.excludeLayers
    }

    public static Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
        mesh.uv = sprite.uv;
        mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);
        return mesh;
    }

    public void ToggleGravity(bool useGravity)
    {
        _rigidbody.useGravity = useGravity;
    }

    public void TogglePhysics(bool usePhysics)
    {
        if (!usePhysics) {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            Debug.Log("zeroing velocity");
        } else {
            Debug.Log("not zeroing velocity");
        }
        _rigidbody.isKinematic = usePhysics;
        Debug.Log("usePhysics: " + usePhysics);
    }

    public void AddForce(Vector3 force)
    {
        _rigidbody.AddForce(force);
    }
}
