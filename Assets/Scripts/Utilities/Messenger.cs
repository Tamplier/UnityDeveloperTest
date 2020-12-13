using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messenger : MonoBehaviour
{
    public static Messenger instance;
    public event Action onBallFell;
    public event Action onLastPlatformReflected;
    public event Action<Platform, float> onNewPlatformCreated;
    public event Action onReturnBallRequested; 

    private void Awake()
    {
        if (instance == null) instance = this;
        else if(instance != this) Destroy(gameObject);
    }

    public void ballFell()
    {
        onBallFell?.Invoke();
    }

    public void lastPlatformReflected()
    {
        onLastPlatformReflected?.Invoke();
    }

    public void newPlatformCreated(Platform platform, float progress)
    {
        onNewPlatformCreated?.Invoke(platform, progress);
    }

    public void requestReturnBall()
    {
        onReturnBallRequested?.Invoke();
    }
}
