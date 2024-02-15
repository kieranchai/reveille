using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public GameObject ring;
    public GameObject slot;
    public GameObject startPos;
    public GameObject targetPos;

    [Header("Player Ring")]
    public int speed;
    public bool rotating;

    [Header("Ring Patterns")]
    public List<GameObject> midRing = new List<GameObject>();
    public int counter;
    public bool solved; //based on slot
    public bool failed; //based on slot

    private bool playedAlertSound = false;

    public enum HackState
    {
        PLAY,
        NEXT,
        WIN,
        LOSE
    }

    public HackState currentHackingState = HackState.PLAY;

    private void Start()
    {
        rotating = true;

        counter = 0;
        solved = false;
        failed = false;
    }

    private void Update()
    {
        switch (currentHackingState)
        {
            case HackState.PLAY:
                RotateRing();
                break;
            case HackState.NEXT:
                CheckCondition();
                break;
            case HackState.WIN:
                WinMinigame();
                break;
            case HackState.LOSE:
                LoseMinigame();
                break;
        }
    }

    public void RotateRing()
    {
        if (!rotating) return;

        if (Input.GetMouseButtonDown(0))
        {
            rotating = false;
            ring.transform.Rotate(0, 0, 0);
            slot.transform.position = Vector2.MoveTowards(startPos.transform.position, targetPos.transform.position, 1000 * Time.deltaTime);

            StartCoroutine(DelayNextState());
        }

        ring.transform.Rotate(0, 0, speed * Time.deltaTime);
    }

    public void CheckCondition()
    {
        //if slot collide with ring - fail and close
        //if slot collide with hole - check if got another ring, if not close
        if (failed)
        {
            currentHackingState = HackState.LOSE;
        }
        else if (solved)
        {
            ++counter;
            solved = false;

            //check if got another ring
            if (counter != midRing.Count)
            {
                midRing[counter - 1].gameObject.SetActive(false);
                //make the ring scale smaller/ move positions closer
                Vector3 scaleChange = new Vector3(0.2f, 0.2f, 0f);
                Vector3 posChange = new Vector3(0.5f, 0f, 0f);

                slot.transform.position = startPos.transform.position;
                ring.transform.localScale -= scaleChange;
                startPos.transform.position -= posChange;
                targetPos.transform.position -= posChange;

                rotating = true;
                currentHackingState = HackState.PLAY;
            }
            else
            {
                currentHackingState = HackState.WIN;
            }
        }
    }

    public void WinMinigame()
    {
        //Play success sound

        if (PlayerManager.instance.hackingTarget.GetComponent<Terminal>().door != null)
        {
            //disable door
            PlayerManager.instance.hackingTarget.GetComponent<Terminal>().door.SetActive(false);
            //door.GetComponent<BoxCollider2D>().enabled = false;
            //animate door
        }
        else if (PlayerManager.instance.hackingTarget.GetComponent<Terminal>().cctv != null)
        {
            //disable cctv temporarily here
        }

        StartCoroutine(DelayExit());
    }

    public void LoseMinigame()
    {
        //Play failed sound
       if (!playedAlertSound)
        {
            playedAlertSound = true;
            PlayerManager.instance.hackingTarget.GetComponent<Terminal>().StartCoroutine(PlayerManager.instance.hackingTarget.GetComponent<Terminal>()._noiseController.ProduceNoiseMultiple(8));
        }

        //Delay timer before being able to hack again
        StartCoroutine(DelayExit());
    }

    public IEnumerator DelayNextState()
    {
        yield return new WaitForSeconds(0.25f);
        currentHackingState = HackState.NEXT;
    }

    public IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(0.25f);
        Destroy(this.gameObject);
        PlayerManager.instance.currentState = PlayerManager.PLAYER_STATE.STILL;
    }
}
