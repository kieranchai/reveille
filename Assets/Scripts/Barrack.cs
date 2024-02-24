using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : MonoBehaviour
{
    public bool fullCapacity;
    public int maxBarrackCapacity;
    public int currentBarrackCapacity;

    private void Start()
    {
        currentBarrackCapacity = 0;
    }

    public void CheckCapacity()
    {
        if (currentBarrackCapacity > maxBarrackCapacity)
        {
            fullCapacity = true;
        }
    }

    public void AddToBarrack(Food food)
    {
        currentBarrackCapacity += food.weight;
        CheckCapacity(); //can exceed max weight once
    }
}
