using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform ballParent;

    private GameObject ball;
    private GameObject fpPos;
    // Start is called before the first frame update
    void Start()
    {
        Messenger.instance.onReturnBallRequested += spawnBall;
        fpPos = GameObject.FindWithTag("FirstPlatformPosition");
    }

    private void spawnBall()
    {
        if(ball == null) ball = Instantiate(ballPrefab, ballParent);
        ball.transform.position = getNewBallPosition();
        ball.gameObject.SetActive(true);
    }

    private Vector3 getNewBallPosition()
    {
        PlatformMoving[] platforms = GameObject.FindGameObjectsWithTag("Platform")
            .Select(p => p.GetComponent<PlatformMoving>())
            .ToArray();
        float minZ = platforms.Min(p => Mathf.Abs(p.transform.position.z - fpPos.transform.position.z));
        PlatformMoving firstPlatform = platforms
            .FirstOrDefault(p => Mathf.Abs(p.transform.position.z - fpPos.transform.position.z) == minZ);
        firstPlatform.GetComponentInChildren<PlatformColliding>().setScoreCount(false);
        Vector3 firstPlatformPos = firstPlatform.transform.position;
        float rollBackTime = Mathf.Abs((fpPos.transform.position.z - firstPlatformPos.z) / firstPlatform.getVelocity().z);
        firstPlatformPos.y += 1;
        firstPlatformPos.z = fpPos.transform.position.z;
        foreach (PlatformMoving platform in platforms) platform.returnBackInTime(rollBackTime);
        return firstPlatformPos;
    }

    private void OnDestroy()
    {
        Messenger.instance.onReturnBallRequested -= spawnBall;
    }
}
