using UnityEngine;

public class GameData
{
    private const string BEST_SCORE = "best_score";
    
    public static void setBestScore(int score)
    {
        PlayerPrefs.SetInt(BEST_SCORE, score);
    }

    public static int getBestScore()
    {
        return PlayerPrefs.GetInt(BEST_SCORE, 0);
    }
}
