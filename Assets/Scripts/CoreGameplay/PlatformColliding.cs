using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class PlatformColliding : MonoBehaviour
{
    public float shakingDistance = 1f;
    public float shakeTime = 0.2f;
    private Vector3 velocity;
    private Transform nextPlatform;
    private bool reflected;
    private bool disableScore;
    private bool shaked;
    private float shakeSpeed;
    private Vector3 shakeDirection;
    private float startPosition;

    private void Awake()
    {
        shakeSpeed = shakingDistance * 2 / shakeTime;
        startPosition = transform.position.y;
    }

    public void init(Vector3 velocity, bool disableScore)
    {
        this.velocity = velocity;
        this.disableScore = disableScore;
        reflected = false;
        nextPlatform = null;
        shaked = false;
        shakeDirection = Vector3.down;
    }

    public void setScoreCount(bool flag)
    {
        disableScore = !flag;
    }

    public bool isScoreCountingEnabled()
    {
        return !disableScore;
    }

    public void setNextPlatform(Transform nextPlatform)
    {
        this.nextPlatform = nextPlatform;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!disableScore) GameController.instance.score++;
        updateColliderSpeed(other.collider);
    }

    private void OnCollisionStay(Collision other)
    {
        updateColliderSpeed(other.collider);
    }

    private void updateColliderSpeed(Collider other)
    {
        if(GameController.instance.isGamePaused) return;
        
        Ball ball = other.GetComponent<Ball>();
        float speedY = 30;
        float speedZ = 50;
        if (nextPlatform != null)
        {
            float nextPlatformTime = Mathf.Abs(other.transform.position.z - nextPlatform.position.z) / -velocity.z;
            speedY = -Physics.gravity.y * nextPlatformTime / 2f;
            speedZ = 0;
        }
        else ball.trail.emitting = true;
        Vector3 vel = new Vector3(0, speedY, speedZ);
        
        if(!reflected)
        {
            ball.rigidbody.velocity = vel;
            reflected = true;
            if(speedZ > 0) GameController.instance.gameOver(false);
        }
    }

    private void Update()
    {
        if(shaked || !reflected || disableScore) return;
        
        transform.Translate(shakeDirection*shakeSpeed * Time.deltaTime);
        if (shakeDirection == Vector3.down && startPosition - transform.position.y >= shakingDistance)
            shakeDirection = Vector3.up;
        else if (shakeDirection == Vector3.up && startPosition - transform.position.y <= 0)
        {
            reflected = true;
            shaked = true;
        }
            
    }
}
