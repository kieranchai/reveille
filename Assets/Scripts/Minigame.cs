using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    private AudioSource _audio;
    public AudioClip insertSlot;

    public GameObject ring;
    public GameObject slot;
    public GameObject startPos;
    public GameObject targetPos;
    public GameObject midRingPrefab;

    [Header("Terminal Variables")]
    public int speed;
    public int rings;

    [Header("Attributes")]
    public bool rotating;
    public GameObject activeRing;
    public List<GameObject> midRingList = new List<GameObject>();
    public int counter;
    public bool inserted;
    public bool solved; //based on slot
    public bool failed; //based on slot

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
        _audio = GetComponent<AudioSource>();

        rotating = true;

        counter = 0;
        inserted = false;
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
            case HackState.LOSE:
                break;
        }
    }

    public void Initialise(int speed, int rings)
    {
        this.speed = speed;
        this.rings = rings;

        CreatingRings();
    }

    public void CreatingRings()
    {
        for (int i = 0; i < rings; i++)
        {
            GameObject newRing = Instantiate(midRingPrefab, transform);

            float scale = i == 0 ? 1.0f : (i == 1 ? 0.75f : 0.5f); //fixed operation up to 3
            int randNum = Random.Range(0, 346);
            newRing.transform.localScale = new Vector3(scale, scale, scale);
            newRing.transform.Rotate(0, 0, randNum);

            midRingList.Add(newRing);
        }
    }

    public void RotateRing()
    {
        if (!rotating) return;

        if (Input.GetMouseButtonDown(0))
        {
            inserted = true;

            _audio.clip = insertSlot;
            _audio.Play();

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
            midRingList[counter].gameObject.SetActive(false);
            ++counter;
            solved = false;

            //check if got another ring
            if (counter != midRingList.Count)
            {
                //make the ring scale smaller/ move positions closer
                Vector3 scaleChange = new Vector3(0.25f, 0.25f, 0f);
                Vector3 posChange = new Vector3(0.4f, 0f, 0f);

                slot.transform.position = startPos.transform.position;
                ring.transform.localScale -= scaleChange;
                ring.transform.TransformPoint(startPos.transform.localPosition - posChange);
                ring.transform.TransformPoint(targetPos.transform.localPosition - posChange);

                inserted = false;
                rotating = true;
                currentHackingState = HackState.PLAY;
            }
            else
            {
                currentHackingState = HackState.WIN;
            }
        }
    }

    public IEnumerator DelayNextState()
    {
        yield return new WaitForSeconds(0.25f);
        currentHackingState = HackState.NEXT;
    }
}
