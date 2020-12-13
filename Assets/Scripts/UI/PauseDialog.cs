using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseDialog : MonoBehaviour
{
    public Text progressText;
    public void continueGame()
    {
        Messenger.instance.requestReturnBall();
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
