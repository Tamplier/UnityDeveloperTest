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
    public AudioSource audioSource;
    public GameObject loader;

    [HideInInspector]
    public bool isGamePaused
    {
        get => paused;
        set
        {
            paused = value;
            if (!paused)
            {
                if(audioSource.isPlaying) audioSource.UnPause();
                else audioSource.Play();
            }
            else audioSource.Pause();
        }
    }
    
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
    private bool paused;

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
        score = 0;
        Messenger.instance.onBallFell += () => gameOver(true);
        Messenger.instance.onLastPlatformReflected += () => gameOver(false);
        Messenger.instance.onAudioAnalysisFinished += preprocessingFinished;
        Messenger.instance.onReturnBackInTime += returnBackInTime;
    }

    private void returnBackInTime(float rollbackTime)
    {
        audioSource.time -= rollbackTime;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isGamePaused = true;
    }

    private void preprocessingFinished()
    {
        loader.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(pauseDialog.gameObject.activeSelf || loader.activeSelf) return;
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0)) 
            isGamePaused = false;
    }

    public void gameOver(bool isContinuePossible)
    {
        if(score > GameData.getBestScore()) GameData.setBestScore(score);
        if (isContinuePossible)
        {
            Vibration.VibratePeek();
            pauseDialog.gameObject.SetActive(true);
            pauseDialog.setProgress(score / (float) generator.levelLength);
            isGamePaused = true;
        }
        else Invoke(nameof(backToMainMenu), 1.5f);
    }

    private void backToMainMenu()
    {
        isGamePaused = true;
        SceneManager.LoadScene("MenuScene");
    }

    private void OnDestroy()
    {
        Messenger.instance.onBallFell -= () => gameOver(true);
        Messenger.instance.onLastPlatformReflected -= () => gameOver(false);
        Messenger.instance.onReturnBackInTime -= returnBackInTime;
    }
}
