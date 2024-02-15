using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerManager;

public class Terminal : MonoBehaviour
{
    private NoiseController _noiseController;
    public bool playable;
    private GameObject minigame;
    private bool playedAlertSound = false;

    [Header("Unlocks")]
    public GameObject door;
    public GameObject cctv;

    [Header("Minigame Difficulty")]
    public int speed;

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
        minigame = Instantiate(Resources.Load<GameObject>("Prefabs/Minigame"), transform.position, Quaternion.identity);
        minigame.GetComponent<Minigame>().Initialise(speed);
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
        StartCoroutine(_noiseController.StopNoise());

        if (door)
        {
            //disable door
            door.SetActive(false);
            //door.GetComponent<BoxCollider2D>().enabled = false;
            //animate door
        }

        if (cctv)
        {
            //disable cctv temporarily here
        }
    }

    private void SoundAlarm()
    {
        if (!playedAlertSound)
        {
            playedAlertSound = true;
            StartCoroutine(_noiseController.ProduceNoiseMultiple(8));
        }
    }
}
