using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Terminal : MonoBehaviour
{
    private NoiseController _noiseController;
    private Animator _anim;
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
        _anim = gameObject.GetComponent<Animator>();   
    }

    private void Update()
    {
        if (_noiseController.isPulsing)
        {
            _anim.SetBool("isPlayingAlarm", true);
        }
        else
        {
            _anim.SetBool("isPlayingAlarm", false);
        }

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
        _anim.SetBool("isOpen", true);
        foreach (GameObject door in doors)
        {
            StartCoroutine(openDoor(door));
            door.GetComponent<SpriteRenderer>().sprite = openedDoor;
            door.GetComponent<NavMeshObstacle>().carving = false;
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
