using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Linq;

public class SpritePhysicsController : MonoBehaviour, IOscControllable, IArtworkController
{

    private List<SpritePhysics> _spritePhysics = new List<SpritePhysics>();
    public bool PhysicsEnabled = false;
    public bool GravityEnabled = false;

    public Artwork Artwork => GetComponent<Artwork>();
    public string OscAddress => $"/artwork/{Artwork.Id}/physics";


    void OnEnable()
    {
        Debug.Log("Enabling Physics on all child sprites");
        var moveables = GetComponentsInChildren<Moveable>();

        foreach(var moveable in moveables)
        {
            if (gameObject == moveable.gameObject) continue;
            if (moveable.GetComponent<SpriteRenderer>() == null) continue;
            
            SpritePhysics spritePhysics = null;
            if (moveable.gameObject.GetComponent<SpritePhysics>() == null) {
                spritePhysics = moveable.gameObject.AddComponent<SpritePhysics>();
            } else {
                spritePhysics = moveable.gameObject.GetComponent<SpritePhysics>();
            }
            _spritePhysics.Add(spritePhysics);
        }

        RegisterEndpoints();
    }

    public void RegisterEndpoints()
    {
        // make these endpoints fill in controller as the gameObject name
        OscManager.Instance.AddEndpoint($"{OscAddress}/togglePhysics", (OscDataHandle dataHandle) => {
            PhysicsEnabled = !PhysicsEnabled;
            Debug.Log("TOGGLING!!!");
            ToggleSpritePhysics(PhysicsEnabled);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/toggleGravity", (OscDataHandle dataHandle) => {
            GravityEnabled = !GravityEnabled;
            ToggleSpriteGravity(true);
        });

        OscManager.Instance.AddEndpoint($"{OscAddress}/explode", (OscDataHandle dataHandle) => {
            Vector3 center = _spritePhysics.Select(x => x.transform.position).Aggregate((acc, x) => acc + x) / _spritePhysics.Count;

            // explode away from center
            foreach(var sprite in _spritePhysics)
            {
                var direction = sprite.transform.position - center;
                sprite.AddForce(direction * dataHandle.GetElementAsFloat(0));
            }
        });
    }


    void ToggleSpritePhysics(bool enabled)
    {
        foreach(var sprite in _spritePhysics)
        {
            sprite.TogglePhysics(enabled);
        }
    }

    void ToggleSpriteGravity(bool enabled)
    {
        foreach(var sprite in _spritePhysics)
        {
            sprite.ToggleGravity(enabled);
        }
    }
}
