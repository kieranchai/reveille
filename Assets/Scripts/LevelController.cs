using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public float levelTimer;

    private void Update()
    {
        if (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
        }
    }
}
