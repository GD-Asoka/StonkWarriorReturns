using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> tutorials;
    private int currentTutorial = 0;

    public void NextTutorial()
    {
        if(currentTutorial == tutorials.Count - 1)
        {
            SceneManager.LoadScene(1);
        }
        tutorials[currentTutorial].SetActive(false);
        currentTutorial++;
        tutorials[currentTutorial].SetActive(true);
    }
}
