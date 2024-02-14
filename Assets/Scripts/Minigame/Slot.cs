using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Ring ringScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minigame Hole"))
        {
            ringScript.midRing[ringScript.counter].GetComponent<SpriteRenderer>().color = Color.green;
            ringScript.solved = true;

            if (ringScript.door != null)
            {
                //disable door
                ringScript.door.gameObject.SetActive(false);
                //ringScript.door.GetComponent<BoxCollider2D>().enabled = false;
            }
            else if (ringScript.cctv != null)
            {
                //disable cctv temporarily here
            }
        }
        else if (collision.gameObject.CompareTag("Minigame Midring"))
        {
            ringScript.midRing[ringScript.counter].GetComponent<SpriteRenderer>().color = Color.red;
            ringScript.failed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minigame Hole") || collision.gameObject.CompareTag("Minigame Midring"))
        {
            ringScript.midRing[ringScript.counter].GetComponent<SpriteRenderer>().color = Color.white;
            ringScript.failed = false;
        }
    }
}
