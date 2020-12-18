using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public GameObject platformStartPosition;
    public GameObject platformEndPosition;
    public GameObject platformFirstPosition;
    public float platformPositionDeviation = 5f;
    public float platformSpeed = 8;
    public float platformFastSpeed = 50f;
    [HideInInspector]public int levelLength;

    private ObjectPool pool;
    private Vector3 platformsVelocity;
    private Vector3 platformsFastVelocity;
    private float waitingTime;
    private Platform prevPlatform;
    private int platformCounter;
    private float[] progressNotifications = {0.2f, 0.4f, 0.6f, 0.8f, 1};
    private float prevProgress;
    private List<SpectralFluxInfo> spectralInfo;
    private float minFlux;
    private float maxFlux;
    private float fastTravelTime;
    private float slowTravelTime;
    private float songLength;
    // Start is called before the first frame update
    void Start()
    {
        Messenger.instance.onReturnBackInTime += returnBackInTime;
        pool = new ObjectPool(platformPrefab, transform, 20);
        platformsVelocity = new Vector3(0, 0, -platformSpeed);
        platformsFastVelocity = new Vector3(0, 0, -platformFastSpeed);
        fastTravelTime =
            (platformStartPosition.transform.position - platformEndPosition.transform.position).magnitude /
            platformFastSpeed;
        slowTravelTime +=
            (platformEndPosition.transform.position - platformFirstPosition.transform.position).magnitude /
            platformSpeed;
    }

    private void returnBackInTime(float rollbackTime)
    {
        waitingTime -= rollbackTime;
    }

    public void setSpectralData(List<SpectralFluxInfo> pointInfo, float songLength)
    {
        this.songLength = songLength;
        spectralInfo = pointInfo.Where(pi => pi.isPeak && pi.prunedSpectralFlux > 0).ToList();
        for (int i = spectralInfo.Count - 1; i > 0; i--)
        {
            if (spectralInfo[i].time - spectralInfo[i - 1].time < 0.3f) spectralInfo.RemoveAt(i);
        }

        maxFlux = spectralInfo.Max(pi => pi.prunedSpectralFlux);
        minFlux = spectralInfo.Min(pi => pi.prunedSpectralFlux);
        levelLength = spectralInfo.Count;
        init();
        spectralInfo.ForEach(si => si.time -= fastTravelTime + slowTravelTime);
        Messenger.instance.requestReturnBall();
        Messenger.instance.audioAnalysisFinished();
    }


    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.isGamePaused) return;
        if(spectralInfo.Count == 0) return;
        
        waitingTime += Time.deltaTime;
        if(spectralInfo[0].time >= waitingTime) return;
        createPlatform(spectralInfo[0].prunedSpectralFlux, platformStartPosition.transform.position.z);
        spectralInfo.RemoveAt(0);
    }

    private Platform createPlatform(float x, float z, bool fastSpeed = true, bool disableScore = false)
    {
        x = -platformPositionDeviation + (x - minFlux) / (maxFlux - minFlux) * platformPositionDeviation * 2;
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
            slowTravelTime,
            disableScore,
            progressNotification);
        Messenger.instance.newPlatformCreated(platform, progress);
        if(prevPlatform != null) prevPlatform.setNextPlatform(platform.transform);
        prevPlatform = platform;
        return platform;
    }

    private void init()
    {
        float currentZ = platformFirstPosition.transform.position.z;
        prevPlatform = createPlatform((minFlux+maxFlux)*0.5f, currentZ,false, true);
        List<SpectralFluxInfo> currentInfos = spectralInfo.Where(si => si.time <= slowTravelTime).ToList();
        currentInfos.ForEach(info =>
        {
            currentZ = platformSpeed * info.time + platformFirstPosition.transform.position.z;
            Debug.Log("pos " + info.time);
            createPlatform(info.prunedSpectralFlux, currentZ);
            spectralInfo.Remove(info);

        });
        currentInfos = spectralInfo
            .Where(si => si.time > slowTravelTime && si.time <= slowTravelTime + fastTravelTime)
            .ToList();
        currentInfos.ForEach(info =>
        {
            currentZ = platformEndPosition.transform.position.z + (info.time - slowTravelTime)*platformFastSpeed;
            Debug.Log("pos " + currentZ);
            createPlatform(info.prunedSpectralFlux, currentZ);
            spectralInfo.Remove(info);
        });
    }

    private void OnDestroy()
    {
        Messenger.instance.onReturnBackInTime -= returnBackInTime;
    }
}
