using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OscJack;
using UnityEngine;

public class SpritePhysicsController : MonoBehaviour, INetworkEndpoint, IArtworkController
{
    private List<SpritePhysics> _allSpritePhysics;
    public bool PhysicsEnabled = false;
    public bool GravityEnabled = false;

    public SegmentedPaintingArtwork Artwork => GetComponent<SegmentedPaintingArtwork>();
    public string Address => $"/artwork/{Artwork.Id}/physics";

    [SerializeField]
    private bool _physicsEnabled = false;

    [SerializeField]
    private bool _gravityEnabled = false;

    // private void OnEnable()
    // {
    //     Register();
    // }

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
        // make these endpoints fill in controller as the gameObject name
        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/physics/togglePhysics",
            (OscDataHandle dataHandle) =>
            {
                Debug.Log("Toggling Physics");
                ToggleSpritePhysics();
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/physics/toggleGravity",
            (OscDataHandle dataHandle) =>
            {
                GravityEnabled = !GravityEnabled;
                ToggleSpriteGravity();
            },
            this
        );

        OscManager.Instance.AddEndpoint(
            $"{baseAddress}/physics/explode",
            (OscDataHandle dataHandle) =>
            {
                if (!_physicsEnabled)
                    return;

                Vector3 center =
                    _allSpritePhysics
                        .Select(x => x.transform.position)
                        .Aggregate((acc, x) => acc + x) / _allSpritePhysics.Count;

                // explode away from center
                foreach (var sprite in _allSpritePhysics)
                {
                    var direction = sprite.transform.position - center;
                    sprite.AddForce(direction * dataHandle.GetElementAsFloat(0));

                    // there is a rigidbody.AddExplosionForce() method that might be useful
                }
            },
            this
        );
    }

    public void Unregister()
    {
        if (OscManager.Instance == null)
            return;
        OscManager.Instance.RemoveAllEndpointsForOwner(this);
        // OscManager.Instance.RemoveEndpoint($"{Address}/togglePhysics");
        // OscManager.Instance.RemoveEndpoint($"{Address}/toggleGravity");
        // OscManager.Instance.RemoveEndpoint($"{Address}/explode");
    }

    private void DisableSpritePhysicsComponents()
    {
        Artwork.ForeachMoveable(
            (moveable) =>
            {
                if (gameObject == moveable.gameObject)
                    return;
                if (moveable.GetComponent<SpriteRenderer>() == null)
                    return;
                var spritePhysics = moveable.gameObject.GetComponent<SpritePhysics>();
                if (spritePhysics != null)
                {
                    spritePhysics.enabled = false;
                }
            }
        );
    }

    private void EnableSpritePhysicsComponents()
    {
        Debug.Log("Adding Physics components");
        _allSpritePhysics = new List<SpritePhysics>();
        Artwork.ForeachMoveable(
            (moveable) =>
            {
                if (gameObject == moveable.gameObject)
                    return;
                if (moveable.GetComponent<SpriteRenderer>() == null)
                    return;

                SpritePhysics spritePhysics = moveable.gameObject.GetComponent<SpritePhysics>();
                if (spritePhysics != null)
                {
                    spritePhysics.enabled = true;
                }
                else
                {
                    spritePhysics = moveable.gameObject.AddComponent<SpritePhysics>();
                    spritePhysics.enabled = true;
                }
                _allSpritePhysics.Add(spritePhysics);
            }
        );
    }

    void ToggleSpritePhysics()
    {
        _physicsEnabled = !_physicsEnabled;

        if (_physicsEnabled)
        {
            EnableSpritePhysicsComponents();
            foreach (var sprite in _allSpritePhysics)
            {
                sprite.SetEnabled(enabled);
            }
        }
        else
        {
            DisableSpritePhysicsComponents();
        }
    }

    void ToggleSpriteGravity()
    {
        if (!_physicsEnabled)
            return;

        foreach (var sprite in _allSpritePhysics)
        {
            // sprite.SetGravityEnabled(enabled);
            sprite.Rigidbody.useGravity = GravityEnabled;
        }
    }
}
