using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SynthCollider : MonoBehaviour
{
    private AudioSource _audioSource;

    private float[] _samples = new float[1024];

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _audioSource.Play();
        }
    }
}
