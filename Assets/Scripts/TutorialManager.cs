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
        tutorials[currentTutorial].SetActive(false);
        currentTutorial++;
        tutorials[currentTutorial].SetActive(true);
    }
}
