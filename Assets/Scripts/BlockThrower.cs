using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockThrower : MonoBehaviour
{

    public GameObject BlockPrefab;

    public Material BlockMaterial;

    public float throwForce = 10f; 

    public int maxBlocks = 1000;

    public Transform _blockParent;

    List<Rigidbody> _blocks = new List<Rigidbody>();

    public List<Texture2D> Textures;

    void Start()
    {
        
    }


    void ThrowBlock()
    {
        var block = Instantiate(BlockPrefab, transform.position, Quaternion.identity);
        // block.GetComponent<SpriteRenderer>().material = BlockMaterial;

        var forward = transform.forward;

        
        // Get the Rigidbody component of the block
        Rigidbody blockRigidbody = block.GetComponent<Rigidbody>();
        
        // Apply the forward force to the block
        // blockRigidbody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        blockRigidbody.velocity = transform.forward * throwForce;
        // block.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, 0, 1) * 1000);

        block.transform.SetParent(_blockParent);

        // select a random texture and assign it to the material

        var texture = Textures[Random.Range(0, Textures.Count)];
        block.GetComponent<Renderer>().material.mainTexture = texture;

        // make the size random
        


        _blocks.Add(blockRigidbody);
    }

    void Update()
    {
        if (Input.GetKey("space"))
        {
            ThrowBlock();
        }


        while (_blocks.Count > maxBlocks)
        {
            Destroy(_blocks[0].gameObject);
            _blocks.RemoveAt(0);
        }
    }
}
