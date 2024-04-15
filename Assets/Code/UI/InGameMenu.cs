using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{

    public GameObject previousLevelButton;
    public GameObject nextLevelButton;
    public GameObject endGameMenu;

    private int currentSceneId;
    private int numberOfScenes;

    // Start is called before the first frame update
    void Start()
    {
        endGameMenu.SetActive(false);

        currentSceneId = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneId == 1)
        {
            previousLevelButton.SetActive(false);
        }

        numberOfScenes = SceneManager.sceneCountInBuildSettings;

        if (currentSceneId == numberOfScenes - 1)
        {
            nextLevelButton.SetActive(false);
        }
    }

    public void DisplayEndGameMenu()
    {
        endGameMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PreviousLevelLoad()
    {
        int currentSceneId = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneId - 1);
    }

    public void NextLevelLoad()
    {
        int currentSceneId = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneId + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
