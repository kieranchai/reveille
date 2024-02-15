using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTest : MonoBehaviour
{
    public MinigameTest minigame;

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
}
