using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> tutorials;
    private int currentTutorial = 0;
    public GameObject pauseMenu, optionsMenu;

    public void NextTutorial()
    {
        tutorials[currentTutorial].SetActive(false);
        currentTutorial++;
        tutorials[currentTutorial].SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if(optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void IncreaseVolume()
    {

    }
    
    public void DecreaseVolume()
    {

    }
}
