using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    private NoiseController _noiseController;
    public bool playable;
    private GameObject minigame;
    private bool playedAlertSound = false;

    [Header("Unlocks")]
    public List<GameObject> doors;

    [Header("Minigame Difficulty")]
    public int speed;
    public int rings;

    private void Awake()
    {
        playable = true;
        _noiseController = transform.Find("Noise").GetComponent<NoiseController>();
    }

    private void Update()
    {
        if (minigame == null) return;

        if (minigame.GetComponent<Minigame>().currentHackingState == Minigame.HackState.WIN)
        {
            Unlock();
            StopHacking();
        }
        else if (minigame.GetComponent<Minigame>().currentHackingState == Minigame.HackState.LOSE)
        {
            SoundAlarm();
            StopHacking();
        }
    }

    public void StartHacking()
    {
        minigame = Instantiate(Resources.Load<GameObject>("Prefabs/Minigame"), Camera.main.transform.GetChild(0).position, Quaternion.identity);
        minigame.GetComponent<Minigame>().Initialise(speed, rings);
    }

    public void StopHacking()
    {
        Destroy(minigame);
        PlayerManager.instance.currentState = PlayerManager.PLAYER_STATE.STILL;
        playedAlertSound = false;
    }

    private void Unlock()
    {
        playable = false;
        StopAllCoroutines();
        _noiseController.StopNoise();

        foreach (GameObject door in doors)
        {
            //disable door
            door.SetActive(false);
            //door.GetComponent<BoxCollider2D>().enabled = false;
            //animate door
        }
    }

    private void SoundAlarm()
    {
        if (!playedAlertSound)
        {
            StopAllCoroutines();
            _noiseController.ResetNoise();
            playedAlertSound = true;
            _noiseController.StartCoroutine(_noiseController.ProduceNoiseTimer());
        }
    }
}
