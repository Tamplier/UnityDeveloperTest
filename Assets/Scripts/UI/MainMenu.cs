using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text scoreText;
    private bool startLoadingMainScene;
    // Start is called before the first frame update
    void Start()
    {
        startLoadingMainScene = false;
        scoreText.text = $"Best score: {GameData.getBestScore()}";
    }

    // Update is called once per frame
    void Update()
    {
        if(startLoadingMainScene) return;

        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            startLoadingMainScene = true;
            SceneManager.LoadScene("MainScene");
        }
    }
}
