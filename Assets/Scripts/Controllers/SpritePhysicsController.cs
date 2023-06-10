using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class SpritePhysicsController : MonoBehaviour, IOscControllable, IArtworkController
{

    private List<SpritePhysics> _allSpritePhysics;
    public bool PhysicsEnabled = false;
    public bool GravityEnabled = false;

    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Index}/physics";


    [SerializeField] private bool _physicsEnabled = false;
    [SerializeField] private bool _gravityEnabled = false;


    void OnEnable()
    {
        RegisterEndpoints();
    }

    void OnDisable()
    {
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/togglePhysics");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/toggleGravity");
        OscManager.Instance.RemoveEndpoint($"{OscAddress}/explode");
    }

    public void RegisterEndpoints()
    {
        // make these endpoints fill in controller as the gameObject name
        OscManager.Instance.AddEndpoint($"{OscAddress}/togglePhysics", (OscDataHandle dataHandle) => {
            Debug.Log("Toggling Physics");
            ToggleSpritePhysics();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/toggleGravity", (OscDataHandle dataHandle) => {
            GravityEnabled = !GravityEnabled;
            ToggleSpriteGravity();
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/explode", (OscDataHandle dataHandle) => {
            if (!_physicsEnabled) return;

            Vector3 center = _allSpritePhysics.Select(x => x.transform.position).Aggregate((acc, x) => acc + x) / _allSpritePhysics.Count;

            // explode away from center
            foreach(var sprite in _allSpritePhysics)
            {
                var direction = sprite.transform.position - center;
                sprite.AddForce(direction * dataHandle.GetElementAsFloat(0));
            }
        });
    }

    private void DisableSpritePhysicsComponents()
    {
        Artwork.ForeachMoveable((moveable) => {
            if (gameObject == moveable.gameObject) return;
            if (moveable.GetComponent<SpriteRenderer>() == null) return;
            var spritePhysics = moveable.gameObject.GetComponent<SpritePhysics>();
            if (spritePhysics != null) {
                spritePhysics.enabled = false;
            }
        });
    }

    private void EnableSpritePhysicsComponents()
    {
        Debug.Log("Adding Physics components");
        _allSpritePhysics = new List<SpritePhysics>();
        Artwork.ForeachMoveable((moveable) => {
            if (gameObject == moveable.gameObject) return;
            if (moveable.GetComponent<SpriteRenderer>() == null) return;
            
            SpritePhysics spritePhysics = moveable.gameObject.GetComponent<SpritePhysics>();
            if (spritePhysics != null) {
                spritePhysics.enabled = true;
            } else {
                spritePhysics = moveable.gameObject.AddComponent<SpritePhysics>();
                spritePhysics.enabled = true;
            }
            _allSpritePhysics.Add(spritePhysics);
        });
    }


    void ToggleSpritePhysics()
    {
        _physicsEnabled = !_physicsEnabled;

        if (_physicsEnabled) {
            EnableSpritePhysicsComponents();
            foreach(var sprite in _allSpritePhysics) {
                sprite.TogglePhysics(enabled);
            }
        } else {
            DisableSpritePhysicsComponents();
        }


    }

    void ToggleSpriteGravity()
    {
        if (!_physicsEnabled) return;
        
        foreach(var sprite in _allSpritePhysics)
        {
            sprite.ToggleGravity(enabled);
        }
    }
}
