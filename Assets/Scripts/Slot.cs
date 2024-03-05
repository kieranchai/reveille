using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Minigame minigame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (minigame.inserted && collision.gameObject.CompareTag("Minigame Midring"))
        {
            minigame.midRingList[minigame.counter].GetComponent<SpriteRenderer>().color = Color.red;
            minigame.failed = true;
        }
        else if (minigame.inserted && !collision.gameObject.CompareTag("Minigame Midring"))
        {
            minigame.midRingList[minigame.counter].GetComponent<SpriteRenderer>().color = Color.green;
            minigame.solved = true;
        }
    }
}
