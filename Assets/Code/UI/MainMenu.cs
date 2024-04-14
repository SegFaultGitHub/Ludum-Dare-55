using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject creditsMenu;
    public GameObject levelsSelectionMenu;
    public GameObject mainMenu;

    private bool creditsOpen;
    private bool levelSelectionOpen;
    private bool mainMenuOpen;

    // Start is called before the first frame update
    void Start()
    {
        creditsOpen = false;
        levelSelectionOpen = false;
        mainMenuOpen = true;
        creditsMenu.SetActive(creditsOpen);
        levelsSelectionMenu.SetActive(levelSelectionOpen);
        mainMenu.SetActive(mainMenuOpen);
    }

    public void LevelsSelectionMenu()
    {
        creditsOpen = false;
        levelSelectionOpen = true;
        mainMenuOpen = false;

        creditsMenu.SetActive(creditsOpen);
        levelsSelectionMenu.SetActive(levelSelectionOpen);
        mainMenu.SetActive(mainMenuOpen);
    }

    public void CreditsMenu()
    {
        creditsOpen = true;
        levelSelectionOpen = false;
        mainMenuOpen = false;
        
        creditsMenu.SetActive(creditsOpen);
        levelsSelectionMenu.SetActive(levelSelectionOpen);
        mainMenu.SetActive(mainMenuOpen);
    }

    public void BackMainMenu()
    {
        creditsOpen = false;
        levelSelectionOpen = false;
        mainMenuOpen = true;

        creditsMenu.SetActive(creditsOpen);
        levelsSelectionMenu.SetActive(levelSelectionOpen);
        mainMenu.SetActive(mainMenuOpen);
    }

    public void LevelLoading(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
