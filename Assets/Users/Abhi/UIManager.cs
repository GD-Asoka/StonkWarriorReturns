using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject credits;
    public GameObject gameOver, winScreen, loseScreen;
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(2);
    }

    public void ToggleCredits()
    {
        if (!credits) return;
        credits.SetActive(!credits.activeInHierarchy);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionsMenu();
            if(credits)
                if(credits.activeInHierarchy)
                    ToggleCredits();
        }
    }

    public void GameOver(bool win)
    {
        gameOver.SetActive(win);
        if (win)
        {
            winScreen.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(false);
        }
    }

    public void ToggleOptionsMenu()
    {
        optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
