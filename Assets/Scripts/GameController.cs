using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerManager;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }

    private string currentScene;
    public LevelController currentLevelController;
    public UIManager currentUIManager;
    public bool isPaused = false;
    public bool isPanning = true;
    public bool gameEnd = false;

    private void Awake()
    {
        instance = this;

        UpdateCurrentControllers();
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        CheckPause();
    }

    public void UpdateCurrentControllers()
    {
        if (currentScene == "Main Menu") return;

        currentScene = SceneManager.GetActiveScene().name;
        currentLevelController = GameObject.Find("Game Manager").GetComponent<LevelController>();
        currentUIManager = GameObject.Find("Game Manager").GetComponent<UIManager>();
    }

    public void CheckPause()
    {
        if (currentScene == "Main Menu") return;
        if (isPanning) return;

        if (Input.GetKeyDown(KeyCode.Escape) && PlayerManager.instance.currentState != PLAYER_STATE.HACKING)
        {
            isPaused = !isPaused;

            if (isPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0.0f;
        currentUIManager.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1.0f;
        currentUIManager.HidePauseMenu();
    }

    public void GameOver()
    {
        isPaused = true;
        gameEnd = true;

        Time.timeScale = 0.0f;
        currentUIManager.ShowGameOverMenu();
    }

    public void GameWon()
    {
        isPaused = true;
        gameEnd = true;

        Time.timeScale = 0.0f;
        currentUIManager.ShowWinMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RetryGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(currentScene);
    }
}
