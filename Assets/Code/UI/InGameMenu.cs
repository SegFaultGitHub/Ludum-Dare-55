using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour {
    public GameObject previousLevelButton;
    public GameObject nextLevelButton;
    public GameObject endGameMenu;

    private int currentSceneId { get => this.OrderedLevels.IndexOf(SceneManager.GetActiveScene().name); }

    [Scene]
    public string MainMenu;
    public List<string> OrderedLevels;

    // Start is called before the first frame update
    private void Start() {
        this.endGameMenu.SetActive(false);
        this.previousLevelButton.SetActive(this.currentSceneId == 0);
        this.nextLevelButton.SetActive(this.currentSceneId == this.OrderedLevels.Count - 1);
    }

    public void DisplayEndGameMenu() => this.endGameMenu.SetActive(true);

    public void BackToMainMenu() => SceneManager.LoadScene(this.MainMenu);

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void PreviousLevelLoad() => SceneManager.LoadScene(this.OrderedLevels[this.currentSceneId - 1]);

    public void NextLevelLoad() => SceneManager.LoadScene(this.OrderedLevels[this.currentSceneId + 1]);

    public void QuitGame() => Application.Quit();
}
