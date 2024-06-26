using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public float levelTimer;
    private float targetScore;
    public int droppedFoodCount;
    private int totalFoodCount;

    [SerializeField] List<Food> levelFood = new List<Food>();
    [SerializeField] List<Transform> foodTable = new List<Transform>();

    private void Awake()
    {
        droppedFoodCount = 0;
        totalFoodCount = levelFood.Count;
    }

    private void Update()
    {
        if (GameController.instance.isPanning) return;
        if (GameController.instance.gameEnd) return;

        if (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            PlayerManager.instance.timePoints = (int)levelTimer;

            if (droppedFoodCount == totalFoodCount) GameController.instance.GameWon();
        }
        else
        {
            levelTimer = 0;
            PlayerManager.instance.timePoints = (int)levelTimer;

            if (PlayerManager.instance.points + PlayerManager.instance.timePoints >= targetScore) GameController.instance.GameWon();
            else GameController.instance.GameOver();
        }
    }

    public void SpawnFood()
    {
        for (int i = 0; i < levelFood.Count; i++)
        {
            GameObject thrownFood = Instantiate(Resources.Load<GameObject>("Prefabs/Food"), foodTable[i].position, Quaternion.identity);
            thrownFood.GetComponent<FoodManager>().SetDefaultFoodData(levelFood[i]);
            targetScore += 0.7f * levelFood[i].maxPoints;
        }

        targetScore += 0.5f * levelTimer;
    }
}
