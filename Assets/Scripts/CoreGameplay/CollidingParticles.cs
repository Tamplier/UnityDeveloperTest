using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidingParticles : MonoBehaviour
{
    private ObjectPool pool;
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        
    }

    private void OnEnable()
    {
        particles.time = 0;
        particles.Play();
    }

    public void init(ObjectPool pool)
    {
        this.pool = pool;
    }

    void OnParticleSystemStopped()
    {
        
        pool.Despawn(gameObject);
    }
}
