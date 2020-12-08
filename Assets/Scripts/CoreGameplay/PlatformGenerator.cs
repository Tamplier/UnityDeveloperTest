using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public GameObject ballPrefab;
    public GameObject platformStartPosition;
    public GameObject platformEndPosition;
    public GameObject platformFirstPosition;
    public float generationDelayMean = 2f;
    public float generationDelayDeviation = 0.7f;
    public float platformPositionDeviation = 5f;
    public float platformSpeed = 8;
    public float platformFastSpeed = 50f;
    [HideInInspector] public int levelLength;
    [HideInInspector] public Gradient platformColors;

    private ObjectPool pool;
    private Vector3 platformsVelocity;
    private Vector3 platformsFastVelocity;
    private float waitingTime;
    private float nextGenerationTime;
    private Platform prevPlatform;
    private int platformCounter;
    private float[] progressNotifications = {0.2f, 0.4f, 0.6f, 0.8f, 1};
    private float prevProgress;
    // Start is called before the first frame update
    void Start()
    {
        pool = new ObjectPool(platformPrefab, transform, 20);
        platformsVelocity = new Vector3(0, 0, -platformSpeed);
        platformsFastVelocity = new Vector3(0, 0, -platformFastSpeed);
        init();
    }
    

    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.isGamePaused) return;
        if(platformCounter >= levelLength) return;
        
        waitingTime += Time.deltaTime;
        if(waitingTime < nextGenerationTime) return;

        waitingTime = 0;
        nextGenerationTime = getNextGenerationTime();
        createPlatform(platformStartPosition.transform.position.z);
    }

    private Platform createPlatform(float z, bool fastSpeed = true, bool disableScore = false)
    {
        float xPos = Random.Range(-platformPositionDeviation, platformPositionDeviation);
        return createPlatform(xPos, z, fastSpeed, disableScore);
    }

    private Platform createPlatform(float x, float z, bool fastSpeed = true, bool disableScore = false)
    {
        GameObject go = pool.Spawn(new Vector3(x, transform.position.y, z));
        platformCounter++;
        float progress = platformCounter / (float)levelLength;
        float progressNotification = progressNotifications.FirstOrDefault(n => n > prevProgress && n <= progress);
        prevProgress = progress;
        
        Platform platform = go.GetComponent<Platform>();
        platform.initPlatform(
            platformsVelocity, 
            platformsFastVelocity, 
            platformEndPosition.transform.position, 
            fastSpeed,
            pool,
            disableScore,
            progressNotification);
        platform.setColor(platformColors.Evaluate(progress));
        if(prevPlatform != null) prevPlatform.setNextPlatform(platform.transform);
        prevPlatform = platform;
        return platform;
    }

    private void spawnBall(Vector3 position)
    {
        GameObject ball = Instantiate(ballPrefab, transform.parent.parent);
        ball.transform.position = position;
    }

    private float getNextGenerationTime()
    {
        return generationDelayMean + Random.Range(-generationDelayDeviation, generationDelayDeviation);
    }

    private void init()
    {
        float currentZ = platformFirstPosition.transform.position.z;
        float time = getNextGenerationTime();
        prevPlatform = createPlatform(0, currentZ,false, true);
        Vector3 firstPlatformPos = prevPlatform.transform.position;
        firstPlatformPos.y += 1;
        spawnBall(firstPlatformPos);
        
        float lastDistance = getNextGenerationTime() * platformSpeed;
        float minDistance = (generationDelayMean - generationDelayDeviation) * platformSpeed;
        platformCounter = 0;
        prevProgress = 0;
        currentZ += time * platformSpeed;
        float fastWayDistance =
            Mathf.Abs(platformEndPosition.transform.position.z - platformStartPosition.transform.position.z) /
            platformFastSpeed;
        fastWayDistance *= platformSpeed;
        while(currentZ <= platformEndPosition.transform.position.z - lastDistance - minDistance + fastWayDistance)
        {
            time = getNextGenerationTime();
            createPlatform(currentZ, false);
            currentZ += time * platformSpeed;
        }
        currentZ = platformEndPosition.transform.position.z - lastDistance + fastWayDistance;
        createPlatform(currentZ, false);
    }
}
