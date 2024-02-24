using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float currentTime;
    public float startingTime;

    private void Start()
    {
        currentTime = startingTime;
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;

        EndLevel();
    }

    public void EndLevel()
    {
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            //end game here
        }
    }
}
