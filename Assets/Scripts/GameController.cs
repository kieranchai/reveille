using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerManager;
using static UnityEngine.GraphicsBuffer;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }

    public string currentScene;
    public LevelController currentLevelController;
    public UIManager currentUIManager;
    public bool isPaused = false;
    public bool isPanning = true;
    public bool gameEnd = false;
    public bool isTutorialScene = false;
    private GameObject tutorialDialogue;
    public bool canInteract = true;

    private void Awake()
    {
        instance = this;

        UpdateCurrentControllers();
        Time.timeScale = 1.0f;
        if (isTutorialScene)
        {
            tutorialDialogue = GameObject.Find("Tutorial Dialogue");
            tutorialDialogue.GetComponent<TMP_Text>().text = string.Empty;
            tutorialDialogue.SetActive(false);
        }
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
            if (currentUIManager.controlsMenu.activeSelf)
            {
                currentUIManager.HideControlsGame();
            }
            else
            {
                isPaused = !isPaused;

                if (isPaused) PauseGame();
                else ResumeGame();
            }
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

        AudioManager.instance.PlaySFX(AudioManager.instance.gameLose);

        Time.timeScale = 0.0f;
        currentUIManager.ShowGameOverMenu();
    }

    public void GameWon()
    {
        isPaused = true;
        gameEnd = true;

        AudioManager.instance.PlaySFX(AudioManager.instance.gameWin);

        Time.timeScale = 0.0f;
        currentUIManager.ShowWinMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
        AudioManager.instance.PlayBGM(AudioManager.instance.levelOneMusic);
    }

    public void ExitGame()
    {
        AudioManager.instance.PlayBGM(AudioManager.instance.mainMenuMusic);
        AudioManager.instance.chaseCounter = 0;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void NextLevel()
    {
        Time.timeScale = 1.0f;
        AudioManager.instance.PlayBGM(AudioManager.instance.levelOneMusic);
        AudioManager.instance.chaseCounter = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FinishGame()
    {
        ExitGame();
    }

    public void RetryGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(currentScene);
        AudioManager.instance.PlayBGM(AudioManager.instance.levelOneMusic);
        AudioManager.instance.chaseCounter = 0;
    }

    public void StartTutorial()
    {
        StartCoroutine(Tutorial());
    }

    IEnumerator Typewriter(string text)
    {
        tutorialDialogue.GetComponent<TMP_Text>().text = "";
        var waitTimer = new WaitForSeconds(.08f);
        foreach (char c in text)
        {
            tutorialDialogue.GetComponent<TMP_Text>().text += c;
            yield return waitTimer;
        }
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator Tutorial()
    {
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("Welcome to Reveille!"));
        yield return StartCoroutine(Typewriter("You can move around by pressing WASD."));
        bool hasMoved = false;
        while (!hasMoved)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                hasMoved = true;
            }
            yield return null;
        }
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("Sprint by holding SHIFT,"));
        yield return StartCoroutine(Typewriter("or sneak around by holding CTRL."));
        bool hasSprintOrSneak = false;
        while (!hasSprintOrSneak)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl))
            {
                hasSprintOrSneak = true;
            }
            yield return null;
        }
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("You might notice that there is a circle around you whenever you move."));
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("This circle is the noise produced by you."));
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("When you sprint, it increases in size, and vice-versa when you sneak around."));
        yield return StartCoroutine(Typewriter("Take note of this circle as enemies will go to your location upon hearing the noise!"));
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("Now, let's talk about the goal of Reveille."));
        canInteract = false;
        GameObject.Find("Main Camera").GetComponent<CameraController>().target = GameObject.Find("Food Spawn Area").transform;
        yield return StartCoroutine(Typewriter("Your goal is collect the food delivery without getting caught."));
        GameObject.Find("Main Camera").GetComponent<CameraController>().target = GameObject.Find("Dropoff").transform;
        yield return StartCoroutine(Typewriter("Afterwards, you have to deliver the food back to a drop off area."));
        yield return StartCoroutine(Typewriter("You can take as many trips as you want, but remember,"));
        yield return StartCoroutine(Typewriter("the more weight you carry on you, the slower you'll move."));
        tutorialDialogue.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<CameraController>().target = PlayerManager.instance.gameObject.transform;
        GameObject.Find("Tutorial Barrier 1").SetActive(false);
        GameObject.Find("Main Camera").GetComponent<CameraController>().target = GameObject.Find("Hacking Spot").transform;
        canInteract = false;
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("Located around the map are these locked doors."));
        yield return StartCoroutine(Typewriter("Unlocking these doors will open up shortcuts and places for you to hide."));
        yield return StartCoroutine(Typewriter("However, failing to unlock them will sound an alarm, alerting all nearby guards to the area."));
        yield return StartCoroutine(Typewriter("You can unlock them by pressing E on the terminals next to them."));
        canInteract = true;
        GameObject.Find("Main Camera").GetComponent<CameraController>().target = PlayerManager.instance.gameObject.transform;
        while (GameObject.Find("Hacking Spot").GetComponent<Terminal>().playable)
        {
            yield return null;
        }
        tutorialDialogue.SetActive(false);
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(true);
        GameObject.Find("Tutorial Barrier 2").SetActive(false);
        yield return StartCoroutine(Typewriter("Great, now you're on your own!"));
        yield return StartCoroutine(Typewriter("Deliver all the food in the stage to complete the tutorial"));
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(false);
        while (PlayerManager.instance.inventory.Count <= 0)
        {
            yield return null;
        }
        tutorialDialogue.SetActive(true);
        yield return StartCoroutine(Typewriter("Food can be thrown using MOUSE1 to produce noise in order to distract guards."));
        yield return StartCoroutine(Typewriter("However, a food's delivery points will decrease everytime it's thrown, so throw them wisely."));
        yield return StartCoroutine(Typewriter("Food can also be dropped using MOUSE2 to be removed from your inventory silently."));
        yield return new WaitForSeconds(1f);
        tutorialDialogue.SetActive(false);

    }
}
