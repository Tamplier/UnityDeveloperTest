using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    private bool fastTravelTargetReached;
    private Vector3 velocity;
    private Vector3 fastVelocity;
    private ObjectPool pool;
    private float fTravelDistance;
    private Vector3 fastTravelTarget;

    public void init(Vector3 velocity, Vector3 fastVelocity, Vector3 posForFastTravel, bool fastSpeed, ObjectPool pool)
    {
        this.velocity = velocity;
        this.fastVelocity = fastVelocity;
        this.pool = pool;
        fastTravelTarget = posForFastTravel;
        fastTravelTargetReached = !fastSpeed;
        fTravelDistance = getDistanceToFTPoint();
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }

    void Update()
    {
        if(GameController.instance.isGamePaused) return;
        
        transform.Translate((fastTravelTargetReached ? velocity : fastVelocity) * Time.deltaTime);
        if(fastTravelTargetReached) return;
        float newFTDistance = getDistanceToFTPoint();
        if (newFTDistance > fTravelDistance) fastTravelTargetReached = true;
        fTravelDistance = newFTDistance;
    }
    
    private float getDistanceToFTPoint()
    {
        return (transform.position - fastTravelTarget).sqrMagnitude;
    }

    private void OnBecameInvisible()
    {
        pool.Despawn(gameObject);
    }

    public void returnBackInTime(float seconds)
    {
        transform.Translate((fastTravelTargetReached ? -velocity : -fastVelocity) * seconds);
        fTravelDistance = getDistanceToFTPoint();
    }

    public float getTimeTillFastTravelTarget()
    {
        if (fastTravelTargetReached) return 0;
        return Mathf.Abs(transform.position.z - fastTravelTarget.z) / -fastVelocity.z;
    }
}
