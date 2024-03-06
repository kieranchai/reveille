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

    void DisplayLevelTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        levelTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void DisplayAlertDropOffFull()
    {

    }

    //public void UpdateCurrentFood(string name)
    //{
    //    foodIndicator.text = name;
    //}

    public void UpdateWeightCount(int currentWeight, int weightLimit)
    {
        weightCount.text = string.Format("Food Carried: {0} / {1} g", currentWeight.ToString(), weightLimit.ToString());
    }

    public void UpdateTotalPoints(int points)
    {
        pointsIndicator.text = string.Format("Points: {0}", points.ToString());
    }

    public void DisplayInteractables(InteractableType interactable)
    {
        switch (interactable)
        {
            case InteractableType.FOOD:
                interactIndicator.text = "Press E to Pickup Food";
                break;
            case InteractableType.HACK:
                interactIndicator.text = "Press E to Hack Terminal";
                break;
            case InteractableType.DROPOFF:
                interactIndicator.text = "Press E to Deposit Food";
                break;
            default:
                interactIndicator.text = string.Empty;
                break;
        }
    }
}
