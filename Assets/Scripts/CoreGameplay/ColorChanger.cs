using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorChanger : MonoBehaviour
{
    public float timeToChangeColor = 0.5f;
    public Gradient[] colorSchemes;
    
    private Color currentEnvironmentColor;
    private Color prevEnvironmentColor;
    private float changeColorTime;
    private Gradient currentGameColors;
    // Start is called before the first frame update
    void Start()
    {
        Messenger.instance.onNewPlatformCreated += changePlatformColor;
        changeColorTime = 0;
        currentGameColors = colorSchemes[Random.Range(0, colorSchemes.Length)];
        currentEnvironmentColor = currentGameColors.Evaluate(0);
        prevEnvironmentColor = currentEnvironmentColor;
        GameController.instance.onProgressChanged += onProgressChanged;
    }

    // Update is called once per frame
    void Update()
    {
        changeColorTime += Time.deltaTime;
        if (changeColorTime >= timeToChangeColor) changeColorTime = timeToChangeColor;
        Color c = Color.Lerp(prevEnvironmentColor, currentEnvironmentColor, changeColorTime / timeToChangeColor);
        setEnvironmentColor(c);
    }

    private void onProgressChanged(float progress)
    {
        prevEnvironmentColor = currentEnvironmentColor;
        currentEnvironmentColor = currentGameColors.Evaluate(progress);
        changeColorTime = 0;
    }

    public void setEnvironmentColor(Color c)
    {
        RenderSettings.skybox.SetColor("_SkyTint", c);
        RenderSettings.skybox.SetColor("_GroundColor", c);
    }

    private void changePlatformColor(Platform plt, float progress)
    {
        plt.setColor(currentGameColors.Evaluate(progress));
    }

    private void OnDestroy()
    {
        GameController.instance.onProgressChanged -= onProgressChanged;
        Messenger.instance.onNewPlatformCreated -= changePlatformColor;
    }
}
