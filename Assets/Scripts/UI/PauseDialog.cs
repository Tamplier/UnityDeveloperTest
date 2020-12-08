using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseDialog : MonoBehaviour
{
    public Text progressText;
    public void continueGame()
    {
        GameObject ball = FindObjectOfType<Ball>(true).gameObject;
        GameObject fpPos = GameObject.FindWithTag("FirstPlatformPosition");
        PlatformMoving[] platforms = GameObject.FindGameObjectsWithTag("Platform")
            .Select(p => p.GetComponent<PlatformMoving>())
            .ToArray();
        float minZ = platforms.Min(p => Mathf.Abs(p.transform.position.z - fpPos.transform.position.z));
        PlatformMoving firstPlatform = platforms
            .FirstOrDefault(p => Mathf.Abs(p.transform.position.z - fpPos.transform.position.z) == minZ);
        firstPlatform.GetComponentInChildren<PlatformColliding>().setScoreCount(false);
        Vector3 firstPlatformPos = firstPlatform.transform.position;
        float rollBackTime = Mathf.Abs((fpPos.transform.position.z - firstPlatformPos.z) / firstPlatform.getVelocity().z);
        Debug.Log(rollBackTime);
        firstPlatformPos.y += 1;
        firstPlatformPos.z = fpPos.transform.position.z;
        ball.transform.position = firstPlatformPos;
        foreach (PlatformMoving platform in platforms) platform.returnBackInTime(rollBackTime);
        
        ball.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void setProgress(float progress)
    {
        progressText.text = $"{(int)(progress * 100)}%";
    }
}
