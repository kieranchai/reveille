using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Minigame minigame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minigame Midring"))
        {
            minigame.midRing[minigame.counter].GetComponent<SpriteRenderer>().color = Color.red;
            minigame.failed = true;
        }
        else if (collision.gameObject.CompareTag("Minigame Hole"))
        {
            minigame.midRing[minigame.counter].GetComponent<SpriteRenderer>().color = Color.green;
            minigame.solved = true;
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Minigame Hole") || collision.gameObject.CompareTag("Minigame Midring"))
    //    {
    //        minigame.midRing[minigame.counter].GetComponent<SpriteRenderer>().color = Color.white;
    //        minigame.failed = false;
    //    }
    //}
}