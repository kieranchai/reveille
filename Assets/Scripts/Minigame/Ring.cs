using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private GameObject ring;

    public GameObject minigame;
    public GameObject slot;
    public GameObject startPos;
    public GameObject targetPos;

    private bool rotating = true;
    private bool activate = false;

    private bool atStart;
    private bool atEnd;

    [Header("Player Ring")]
    public int speed;

    [Header("Ring Patterns")]
    public GameObject[] midRing;
    public int counter;
    public bool solved;
    public bool failed;

    [Header("Unlocks")]
    public GameObject door;
    public GameObject cctv;

    private void Start()
    {
        ring = this.gameObject;

        atStart = true;
        atEnd = false;

        counter = 0;
        solved = false;
        failed = false;
    }

    private void Update()
    {
        if (counter == midRing.Length)
        {
            //can end here or ln 109
            return;
        }
        else
        {
            //standard rotation
            if (rotating && !activate)
            {
                ring.transform.Rotate(0, 0, speed * Time.deltaTime);
            }

            #region Go to end pos
            if (atStart && Input.GetMouseButtonDown(0))
            {
                activate = true;
            }

            if (atStart && activate)
            {
                slot.transform.position = Vector2.MoveTowards(slot.transform.position, targetPos.transform.position, 10 * Time.deltaTime);

                if (slot.transform.position == targetPos.transform.position)
                {
                    atStart = false;
                    atEnd = true;
                    rotating = false;
                    //checks if succeed at slot script
                }
            }
            #endregion

            #region Solved a ring
            if (atEnd && solved)
            {
                activate = false;
            }
            else if (atEnd && failed)
            {
                activate = false;
            }
            #endregion

            #region Go back to rotating
            if (atEnd && !activate)
            {
                slot.transform.position = Vector2.MoveTowards(slot.transform.position, startPos.transform.position, 10 * Time.deltaTime);

                if (slot.transform.position == startPos.transform.position)
                {
                    atStart = true;
                    atEnd = false;
                    rotating = true;

                    if (counter < midRing.Length && solved)
                    {
                        ++counter;
                        //make the ring scale smaller/move positions closer
                        Vector3 scaleChange = new Vector3(0.25f, 0.25f, 0f);
                        Vector3 slotScaleChange = new Vector3(0.25f, 0f, 0f);
                        Vector3 posChange = new Vector3(0.5f, 0f, 0f);
                        this.transform.localScale -= scaleChange;
                        slot.transform.localScale -= slotScaleChange;
                        startPos.transform.position -= posChange;
                        targetPos.transform.position -= posChange;
                    }
                    else
                    {
                        ring.transform.Rotate(0, 0, speed * Time.deltaTime);
                    }
                }
                else if (failed)
                {
                    slot.transform.position = startPos.transform.position;
                    minigame.gameObject.SetActive(false);
                }
            }
            #endregion
        }
    }
}
