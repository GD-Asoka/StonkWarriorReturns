using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;
    public DifficultySettingsScriptableObject difficultySettings;
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
        SceneManager.LoadScene("GameplayScene");
    }

    public void PlayerWon()
    {

    }

    public void PlayerLost()
    {

    }
}
