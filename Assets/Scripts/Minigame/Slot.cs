using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject midRing;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hole")
        {
            Debug.Log("A");
            midRing.GetComponent<SpriteRenderer>().color = new Color32(0, 255, 0, 255);
        }
        else if (collision.gameObject.name == "Mid Ring 1")
        {
            Debug.Log("B");
            midRing.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 255);
        }
    }
}
