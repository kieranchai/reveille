using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Terminal : MonoBehaviour
{
    private NoiseController _noiseController;
    public bool playable;
    private GameObject minigame;
    private bool playedAlertSound = false;
    public Sprite openedDoor;
    public bool isVerticle;
    private Vector3 offset;

    [Header("Unlocks")]
    public List<GameObject> doors;

    [Header("Minigame Difficulty")]
    public int speed;
    public int rings;

    private void Awake()
    {
        if (isVerticle)
        {
            offset = new Vector3(0, 2, 0);
        }else
        {
            offset = new Vector3(2, 0, 0);
        }

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
            /*door.SetActive(false);*/
            StartCoroutine(openDoor(door));
            door.GetComponent<SpriteRenderer>().sprite = openedDoor;
            door.GetComponent<NavMeshObstacle>().carving = false;
            //door.GetComponent<BoxCollider2D>().enabled = false;
            //animate door
        }
    }

    IEnumerator openDoor(GameObject Door)
    {
        Vector3 target = Door.transform.position - offset;

        while (Vector3.SqrMagnitude(Door.transform.position - target) >= 0.05f)
        {
            Door.transform.position = Vector3.MoveTowards(Door.transform.position, target, 2 * Time.deltaTime);
            yield return null;
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
