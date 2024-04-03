using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DropOff : MonoBehaviour
{
    public int maxCapacity; // MAX WEIGHT
    public int currentCapacity = 0;
    public List<Food> droppedFood = new List<Food>();

    public Light2D _lightSource;
    public AudioClip dropOffToggle;

    private void Start()
    {
        GetComponent<AudioSource>().clip = dropOffToggle;
        _lightSource = GetComponent<Light2D>();
        currentCapacity = 0;
    }

    private void Update()
    {
        ChangeLight();
    }

    public bool DepositFood(Food foodItem)
    {
        if (currentCapacity + foodItem.weight > maxCapacity)
        {
            return false;
        };

        droppedFood.Add(foodItem);
        currentCapacity += foodItem.weight;
        GameController.instance.currentLevelController.droppedFoodCount++;
        return true;
    }

    public void ChangeLight()
    {
        if (currentCapacity < maxCapacity) return;

        _lightSource.color = Color.red;
        Animator animator = GetComponent<Animator>();
        animator.SetBool("fullCapacity", true);
    }
}
