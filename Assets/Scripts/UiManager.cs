using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private LevelManager levelManager;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI barrackAddErrorText;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        EditLevelTimerText();
    }

    public void EditLevelTimerText()
    {
        timerText.text = "Timer: " + levelManager.currentTime.ToString("0") + " sec";

        if (levelManager.currentTime <= 60f)
        {
            timerText.color = Color.red; // display time in red if < 60s
        }
    }

    public IEnumerator BarrackAddErrorText()
    {
        barrackAddErrorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        barrackAddErrorText.gameObject.SetActive(false);
    }
}
