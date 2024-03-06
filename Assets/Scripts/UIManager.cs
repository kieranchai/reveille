using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelTimer;
    [SerializeField] private TMP_Text pointsIndicator;
    [SerializeField] private TMP_Text interactIndicator;
    [SerializeField] private TMP_Text weightCount;
    [SerializeField] private TMP_Text foodIndicator;

    public enum InteractableType
    {
        FOOD,
        HACK,
        DROPOFF,
        NULL
    }

    private void Update()
    {
        DisplayLevelTime(GameController.instance.currentLevelController.levelTimer);
    }

    public void DisplayLevelTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        levelTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void DisplayAlertDropOffFull()
    {

    }

    public void UpdateWeightCount(int currentWeight, int weightLimit)
    {
        weightCount.text = $"Inventory Weight: {currentWeight}/{weightLimit}g";
    }

    public void UpdateTotalPoints(int points)
    {
        pointsIndicator.text = $"Points: {points}";
    }

    public void UpdateDisplayInteractables()
    {
        if (PlayerManager.instance.nearbyFood.Count > 0)
        {
            interactIndicator.text = $"Press E to pick up {PlayerManager.instance.nearbyFood[0].GetComponent<FoodManager>().foodName.ToUpper()}";
            return;
        }

        if (PlayerManager.instance.hackingTarget)
        {
            interactIndicator.text = "Press E to hack door";
            return;
        }

        if (PlayerManager.instance.foodDropOffTarget)
        {
            interactIndicator.text = "Press E to deposit food";
            return;
        }

        interactIndicator.text = string.Empty;
    }
}
