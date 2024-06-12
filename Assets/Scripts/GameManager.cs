using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;
    public DifficultySettingsScriptableObject difficultySettings;
    private List<TraderScript> enemyTraders = new List<TraderScript>();

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameplayScene1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Initialize()
    {
        enemyTraders = FindEnemyTraders();
    }

    private List<TraderScript> FindEnemyTraders()
    {
        TraderScript[] traders = FindObjectsByType<TraderScript>(FindObjectsSortMode.None);
        List<TraderScript> enemyTraders = new List<TraderScript>();
        for (int t = 0; t < traders.Length; t++)
        {
            if (!traders[t].isPlayer)
            {
                enemyTraders.Add(traders[t]);
            }
        }
        return enemyTraders;
    }

    public void PlayerWon()
    {
        UIManager.instance.GameOver(true);
        StockPriceManager.INSTANCE.EndGame();
        for (int t = 0; t < enemyTraders.Count; t++)
        {
            if (!enemyTraders[t].boughtOut)
            {
                return;
            }
        }
    }
    
    public void PlayerLost()
    {
        UIManager.instance.GameOver(false);
        StockPriceManager.INSTANCE.EndGame();
    }    
}
