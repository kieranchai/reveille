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
    }

    public void UpdateCurrentControllers()
    {
        currentScene = SceneManager.GetActiveScene().name;
        currentLevelController = GameObject.Find("Game Manager").GetComponent<LevelController>();
        currentUIManager = GameObject.Find("Game Manager").GetComponent<UIManager>();
    }
}
