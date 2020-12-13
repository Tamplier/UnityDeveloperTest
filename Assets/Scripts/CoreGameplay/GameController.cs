using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public Text scoreText;
    public PauseDialog pauseDialog;
    public PlatformGenerator generator;
    public int minLevelLength = 10;
    public int maxLevelLength = 50;

    [HideInInspector] public bool isGamePaused;
    public event Action<float> onProgressChanged; 

    public int score
    {
        get => _score;
        set
        {
            _score = value;
            scoreText.text = _score.ToString();
            onProgressChanged?.Invoke(_score / (float)generator.levelLength);
        }
    }

    private int _score;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Vibration.Init();
        isGamePaused = true;
        generator.levelLength = Random.Range(minLevelLength, maxLevelLength + 1);
        score = 0;
        Messenger.instance.onBallFell += () => gameOver(true);
        Messenger.instance.onLastPlatformReflected += () => gameOver(false);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isGamePaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(pauseDialog.gameObject.activeSelf) return;
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0)) 
            isGamePaused = false;
    }

    public void gameOver(bool isContinuePossible)
    {
        if(score > GameData.getBestScore()) GameData.setBestScore(score);
        isGamePaused = true;
        if (isContinuePossible)
        {
            Vibration.VibratePeek();
            pauseDialog.gameObject.SetActive(true);
            pauseDialog.setProgress(score / (float) generator.levelLength);
        }
        else Invoke(nameof(backToMainMenu), 1.5f);
    }

    private void backToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void OnDestroy()
    {
        Messenger.instance.onBallFell -= () => gameOver(true);
        Messenger.instance.onLastPlatformReflected -= () => gameOver(false);
    }
}
