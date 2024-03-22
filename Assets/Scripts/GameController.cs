using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }

    private string currentScene;
    public LevelController currentLevelController;
    public UIManager currentUIManager;
    public bool isPaused = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        UpdateCurrentControllers();
    }

    private void Update()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
        {
            UpdateCurrentControllers();
        }

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            if (isPaused) PauseGame();
            else ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        // Display pause UI
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        // Display pause UI
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
