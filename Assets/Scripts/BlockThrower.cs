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

    public float throwRadius = 0.5f;

    public float blockLifetime = 10f;

    public float blockSizeRange = 0.5f;

    void Start()
    {
        
    }


    void ThrowBlock()
    {
        var randomPos = Random.insideUnitCircle * throwRadius;

        var block = Instantiate(BlockPrefab, randomPos, Quaternion.identity);

        block.transform.localScale *= Random.Range(1f - blockSizeRange, 1f + blockSizeRange);
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

        StartCoroutine(DestroyBlock(blockRigidbody, blockLifetime));
    }

    private IEnumerator DestroyBlock(Rigidbody block, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(block.gameObject);
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
