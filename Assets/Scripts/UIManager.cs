using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelTimer;

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
}
