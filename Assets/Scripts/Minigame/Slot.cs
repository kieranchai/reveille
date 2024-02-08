using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject midRing;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Minigame Hole"))
        {
            midRing.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (collision.gameObject.CompareTag("Minigame Midring"))
        {
            midRing.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
