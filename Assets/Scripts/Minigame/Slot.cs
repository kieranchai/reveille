using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Ring ringScript;

    private void Start()
    {
        ringScript = FindObjectOfType<Ring>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minigame Hole"))
        {
            ringScript.midRing[ringScript.counter].GetComponent<SpriteRenderer>().color = Color.green;
            ringScript.solved = true;
        }
        else if (collision.gameObject.CompareTag("Minigame Midring"))
        {
            ringScript.midRing[ringScript.counter].GetComponent<SpriteRenderer>().color = Color.red;
            //play alert sound/bool
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minigame Hole") || collision.gameObject.CompareTag("Minigame Midring"))
        {
            ringScript.midRing[ringScript.counter].GetComponent<SpriteRenderer>().color = Color.white;
            ringScript.solved = false;
        }
    }
}
