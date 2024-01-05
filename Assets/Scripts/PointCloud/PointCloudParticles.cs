using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudParticles : MonoBehaviour
{
    private PointCloudArtwork _artwork;
    public float sampleRatio = 0.01f;

    public void LoadArtwork(string artworkName)
    {
        _artwork = new PointCloudArtwork(artworkName);

        var particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            var main = particleSystem.main;
            int totalParticles  = Mathf.FloorToInt(_artwork.Pixels.Length * sampleRatio);
            main.maxParticles = totalParticles;
            main.startLifetime = 1000f;
            main.startSpeed = 0f;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[
                particleSystem.main.maxParticles
            ];
            particleSystem.Clear();
            particleSystem.Emit(particles.Length);
            int numParticlesAlive = particleSystem.GetParticles(particles);
            int numSkips = Mathf.FloorToInt(1 / sampleRatio);

            Vector3 posMult = new Vector3(0.01f, 1f, 0.01f);

            for (int i = 0; i < numParticlesAlive; i+=numSkips)
            {
                var pixel = _artwork.Pixels[i];
                particles[i].position = Vector3.Scale(pixel.position, posMult);
                particles[i].startColor = pixel.color;
                particles[i].startSize = 0.5f;
            }

            particleSystem.SetParticles(particles, numParticlesAlive);
        }
    }
}
