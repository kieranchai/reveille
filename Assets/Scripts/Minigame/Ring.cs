using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private GameObject ring;
    private GameObject slot;
    private GameObject startPos;
    private GameObject targetPos;

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

    private void Start()
    {
        ring = this.gameObject;
        slot = this.gameObject.transform.GetChild(0).gameObject;
        startPos = this.gameObject.transform.GetChild(1).gameObject;
        targetPos = this.gameObject.transform.GetChild(2).gameObject;

        atStart = true;
        atEnd = false;

        counter = 0;
        solved = false;
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
                //timer?
                activate = false;
                midRing[counter].gameObject.SetActive(false);
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
                    ++counter;

                    if (counter < midRing.Length)
                    {
                        //make the ring scale smaller/move positions closer
                        Vector3 scaleChange = new Vector3(0.25f, 0.25f, 0f);
                        Vector3 posChange = new Vector3(0.5f, 0f, 0f);
                        this.transform.localScale -= scaleChange;
                        slot.transform.localScale -= scaleChange;
                        startPos.transform.position -= posChange;
                        targetPos.transform.position -= posChange;
                    }
                    else
                    {
                        this.gameObject.SetActive(false);
                        //can end here or ln44
                    }
                }
            }
            #endregion
        }
    }
}
