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
    public float timeToChangeColor = 0.5f;
    public Gradient[] colorSchemes;

    [HideInInspector] public bool isGamePaused;

    public int score
    {
        get => _score;
        set
        {
            _score = value;
            scoreText.text = _score.ToString();
            prevEnvironmentColor = currentEnvironmentColor;
            currentEnvironmentColor = currentGameColors.Evaluate(_score / (float)generator.levelLength);
            changeColorTime = 0;
        }
    }

    private int _score;
    private Color currentEnvironmentColor;
    private Color prevEnvironmentColor;
    private float changeColorTime;
    private Gradient currentGameColors;

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
        currentGameColors = colorSchemes[Random.Range(0, colorSchemes.Length)];
        currentEnvironmentColor = currentGameColors.Evaluate(0);
        prevEnvironmentColor = currentEnvironmentColor;
        isGamePaused = true;
        generator.levelLength = Random.Range(minLevelLength, maxLevelLength + 1);
        generator.platformColors = currentGameColors;
        score = 0;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        isGamePaused = true;
    }

    // Update is called once per frame
    void Update()
    {
        changeColorTime += Time.deltaTime;
        if (changeColorTime >= timeToChangeColor) changeColorTime = timeToChangeColor;
        Color c = Color.Lerp(prevEnvironmentColor, currentEnvironmentColor, changeColorTime / timeToChangeColor);
        setEnvironmentColor(c);
        if(pauseDialog.gameObject.activeSelf) return;
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) isGamePaused = false;
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

    public void setEnvironmentColor(Color c)
    {
        RenderSettings.skybox.SetColor("_SkyTint", c);
        RenderSettings.skybox.SetColor("_GroundColor", c);
    }

    private void backToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
