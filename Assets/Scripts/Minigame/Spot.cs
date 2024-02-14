using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour
{
    public GameObject minigame;

    public bool isPlayable;
    public bool isNearPlayer;

    private void Start()
    {
        isPlayable = true;
        isNearPlayer = false;
    }

    private void Update()
    {
        if (!isPlayable)
        {
            isNearPlayer = false;
            if (PlayerManager.instance.nearbyHackingSpots.Contains(this.gameObject)) PlayerManager.instance.nearbyHackingSpots.Remove(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isNearPlayer && isPlayable)
        {
            isNearPlayer = true;
            if (!PlayerManager.instance.nearbyHackingSpots.Contains(this.gameObject)) PlayerManager.instance.nearbyHackingSpots.Add(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isNearPlayer && isPlayable)
        {
            isNearPlayer = false;
            if (PlayerManager.instance.nearbyHackingSpots.Contains(this.gameObject)) PlayerManager.instance.nearbyHackingSpots.Remove(this.gameObject);
        }
    }
}
