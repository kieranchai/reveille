using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameSlot : MonoBehaviour
{
    public Minigame minigame;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (minigame.inserted && collision.gameObject.CompareTag("Minigame Midring"))
        {
            minigame.midRingList[minigame.counter].GetComponent<SpriteRenderer>().color = Color.red;
            minigame.failed = true;
        }
        else if (minigame.inserted && !collision.gameObject.CompareTag("Minigame Midring") && collision.gameObject.CompareTag("Minigame Hole"))
        {
            minigame.midRingList[minigame.counter].GetComponent<SpriteRenderer>().color = Color.green;
            minigame.solved = true;
        }

        if (minigame.solved && minigame.failed)
        {
            minigame.midRingList[minigame.counter].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
