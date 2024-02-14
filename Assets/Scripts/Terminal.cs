using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    public GameObject minigame;

    [Header("Unlocks")]
    public bool playable;
    public GameObject door;
    public List<GameObject> cctv = new List<GameObject>();

    private void Start()
    {
        playable = true;
    }

    private void Update()
    {
        if (!door.activeSelf)
        {
            //permanent disable
            playable = false;
        }
        // else if (cctv disabled, make playable false for x seconds, then make playable true)
    }
}
