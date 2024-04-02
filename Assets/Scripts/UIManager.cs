using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject playerHud;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOverMenu;
    [SerializeField] public GameObject controlsMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject winMenu;

    private GameObject controlsInteract;
    private GameObject controlsScroll;
    private GameObject controlsThrow;
    private GameObject controlsDrop;
    private GameObject inventoryDisplay;
    private GameObject inventory;
    private GameObject[] inventoryItems;
    private GameObject inventoryItemInfo;
    private GameObject selectedFood;
    private GameObject selectedFoodInfo;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu") return;

        controlsInteract = playerHud.transform.Find("Controls").Find("Interact").gameObject;
        controlsInteract.SetActive(false);
        controlsScroll = playerHud.transform.Find("Controls").Find("Scroll").gameObject;
        controlsScroll.SetActive(false);
        controlsThrow = playerHud.transform.Find("Controls").Find("Throw").gameObject;
        controlsThrow.SetActive(false);
        controlsDrop = playerHud.transform.Find("Controls").Find("Drop").gameObject;
        controlsDrop.SetActive(false);

        inventoryDisplay = playerHud.transform.Find("Inventory Display").gameObject;
        inventory = inventoryDisplay.transform.Find("Inventory").gameObject;
        inventoryItems = new GameObject[inventory.transform.childCount];
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            inventoryItems[i] = inventory.transform.GetChild(i).gameObject;
        }
        inventoryItemInfo = inventoryDisplay.transform.Find("Item Info").gameObject;
        inventoryDisplay.SetActive(false);
        inventoryItemInfo.SetActive(false);

        selectedFood = playerHud.transform.Find("Selected Food").gameObject;
        selectedFoodInfo = selectedFood.transform.Find("Info").gameObject;
        selectedFoodInfo.SetActive(false);

        HideWinMenu();
        HideGameOverMenu();
        HidePauseMenu();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu") return;
        DisplayLevelTime(GameController.instance.currentLevelController.levelTimer);
    }

    #region Player HUD
    public void DisplayLevelTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        playerHud.transform.Find("Level Timer").Find("Timer").GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateCurrentFood()
    {
        if (PlayerManager.instance.inventory.Count < 2) controlsScroll.SetActive(false);
        else controlsScroll.SetActive(true);

        if (PlayerManager.instance.inventory.Count > 0)
        {
            controlsDrop.SetActive(true);
            controlsThrow.SetActive(true);

            selectedFood.transform.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/UI/Food/{PlayerManager.instance.inventory[PlayerManager.instance.currentSelectedFood].foodName}");
            selectedFood.transform.Find("Name").gameObject.GetComponent<TMP_Text>().text = $"{PlayerManager.instance.inventory[PlayerManager.instance.currentSelectedFood].foodName}";

            selectedFoodInfo.SetActive(true);
            selectedFoodInfo.transform.Find("Points").gameObject.GetComponent<TMP_Text>().text = $"{PlayerManager.instance.inventory[PlayerManager.instance.currentSelectedFood].currentPoints}";
            selectedFoodInfo.transform.Find("Weight").gameObject.GetComponent<TMP_Text>().text = $"{PlayerManager.instance.inventory[PlayerManager.instance.currentSelectedFood].weight} <color=#C9A610>G</color>";
        }
        else
        {
            controlsDrop.SetActive(false);
            controlsThrow.SetActive(false);
            selectedFoodInfo.SetActive(false);
            selectedFood.transform.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/No_Food");
            selectedFood.transform.Find("Name").gameObject.GetComponent<TMP_Text>().text = string.Empty;
        }
    }

    public void UpdateWeightCount(int currentWeight, int weightLimit)
    {
        playerHud.transform.Find("Controls").Find("Open Inventory").Find("Weight Limit").gameObject.GetComponent<TMP_Text>().text = $"{currentWeight}/{weightLimit} <color=#C9A610>G</color>";
    }

    public void DisplayInventory()
    {
        playerHud.transform.Find("Controls").Find("Open Inventory").gameObject.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/TAB_Pressed");

        UpdateInventorySelectedFood();
        inventoryDisplay.SetActive(true);
        DisplaySelectedFoodInfo();
    }

    public void UpdateInventorySelectedFood()
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (PlayerManager.instance.inventory.ElementAtOrDefault(i) == null)
            {
                inventoryItems[i].transform.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/No_Food");
            }
            else inventoryItems[i].transform.Find("Image").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/UI/Food/{PlayerManager.instance.inventory[i].foodName}");

            if (PlayerManager.instance.currentSelectedFood == i && PlayerManager.instance.inventory.Count > 0) inventoryItems[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/Inventory_Equipped");
            else inventoryItems[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/Inventory_Default");
        }

        if (inventory.activeInHierarchy) DisplaySelectedFoodInfo();
    }

    public void DisplaySelectedFoodInfo()
    {
        if (PlayerManager.instance.inventory.Count <= 0)
        {
            inventoryItemInfo.SetActive(false);
            return;
        }

        inventoryItemInfo.SetActive(true);
        inventoryItemInfo.transform.Find("Name").gameObject.GetComponent<TMP_Text>().text = $"{PlayerManager.instance.inventory[PlayerManager.instance.currentSelectedFood].foodName}";
        inventoryItemInfo.transform.Find("Points").gameObject.GetComponent<TMP_Text>().text = $"{PlayerManager.instance.inventory[PlayerManager.instance.currentSelectedFood].currentPoints}";
        inventoryItemInfo.transform.Find("Weight").gameObject.GetComponent<TMP_Text>().text = $"{PlayerManager.instance.inventory[PlayerManager.instance.currentSelectedFood].weight} <color=#C9A610>G</color>";
    }

    public void HideInventory()
    {
        playerHud.transform.Find("Controls").Find("Open Inventory").gameObject.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/TAB_Default");
        inventoryDisplay.SetActive(false);
    }

    public void UpdateTotalPoints(int points)
    {
        playerHud.transform.Find("Points").Find("Points").GetComponent<TMP_Text>().text = $"{points}";
    }

    public void UpdateDisplayInteractables()
    {
        if (PlayerManager.instance.nearbyFood.Count > 0)
        {
            controlsInteract.SetActive(true);
            controlsInteract.transform.Find("Text").GetComponent<TMP_Text>().text = $"Pick up {PlayerManager.instance.nearbyFood[0].GetComponent<FoodManager>().foodName.ToUpper()}";
            return;
        }

        if (PlayerManager.instance.hackingTarget && PlayerManager.instance.hackingTarget.GetComponent<Terminal>().playable)
        {
            controlsInteract.SetActive(true);
            controlsInteract.transform.Find("Text").GetComponent<TMP_Text>().text = "Hack door";
            return;
        }

        if (PlayerManager.instance.foodDropOffTarget && PlayerManager.instance.inventory.Count > 0)
        {
            controlsInteract.SetActive(true);
            controlsInteract.transform.Find("Text").GetComponent<TMP_Text>().text = "Deposit food";
            return;
        }

        if (controlsInteract.activeInHierarchy) controlsInteract.SetActive(false);
    }

    public void SetInteractKeyPressed()
    {
        controlsInteract.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/E_Pressed");
    }

    public void SetInteractKeyDefault()
    {
        controlsInteract.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/E_Default");
    }

    public void SetInteractHacking()
    {
        controlsInteract.transform.Find("Text").GetComponent<TMP_Text>().text = "Stop hacking";
    }

    public void SetSprintKeyPressed()
    {
        playerHud.transform.Find("Controls").Find("Sprint").gameObject.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/SHIFT_Pressed");
    }

    public void SetSprintKeyDefault()
    {
        playerHud.transform.Find("Controls").Find("Sprint").gameObject.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/SHIFT_Default");
    }

    public void SetSneakKeyPressed()
    {
        playerHud.transform.Find("Controls").Find("Sneak").gameObject.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/CTRL_Pressed");
    }

    public void SetSneakKeyDefault()
    {
        playerHud.transform.Find("Controls").Find("Sneak").gameObject.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/CTRL_Default");
    }
    #endregion

    #region Menus
    public void HideAllUI()
    {
        canvas.SetActive(false);
    }

    public void ShowAllUI()
    {
        canvas.SetActive(true);
    }

    public void HidePauseMenu()
    {
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void HideGameOverMenu()
    {
        gameOverMenu.SetActive(false);
    }

    public void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    public void HideWinMenu()
    {
        winMenu.SetActive(false);
    }

    public void ShowWinMenu()
    {
        winMenu.SetActive(true);
        winMenu.transform.Find("Scores").Find("Food").Find("Food Score").GetComponent<TMP_Text>().text = $"{PlayerManager.instance.points}";
        winMenu.transform.Find("Scores").Find("Time").Find("Time Score").GetComponent<TMP_Text>().text = $"{PlayerManager.instance.timePoints}";
        winMenu.transform.Find("Scores").Find("Overall").Find("Overall Score").GetComponent<TMP_Text>().text = $"{PlayerManager.instance.points + PlayerManager.instance.timePoints}";
    }

    public void ShowControlsMainMenu()
    {
        controlsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void HideControlsMainMenu()
    {
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ShowControlsGame()
    {
        controlsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void HideControlsGame()
    {
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    #endregion
}
