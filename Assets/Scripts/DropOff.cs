using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOff : MonoBehaviour
{
    public int maxCapacity; // MAX WEIGHT
    private int currentCapacity = 0;
    public List<Food> droppedFood = new List<Food>();

    public bool DepositFood(Food foodItem)
    {
        if (currentCapacity + foodItem.weight >= maxCapacity)
        {
            return false;
        };

        droppedFood.Add(foodItem);
        currentCapacity += foodItem.weight;
        GameController.instance.currentLevelController.droppedFoodCount++;
        return true;
    }
}
