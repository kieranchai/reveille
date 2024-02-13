using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVtoSO
{
    private static string playerCSVPath = "/Assets/Editor/CSVs/Player.csv";
    private static string foodCSVPath = "/Assets/Editor/CSVs/Food.csv";
    private static string enemyCSVPath = "/Assets/Editor/CSVs/Enemy.csv";

    [MenuItem("Utilities/Generate Player")]
    public static void GeneratePlayer()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + playerCSVPath);
        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 3)
            {
                return;
            }

            Player player = ScriptableObject.CreateInstance<Player>();
            player.id = int.Parse(splitData[0]);
            player.baseMovementSpeed = float.Parse(splitData[1]);
            player.inventoryWeightLimit = int.Parse(splitData[2]);
            AssetDatabase.CreateAsset(player, $"Assets/Resources/Data/Player/{player.id}.asset");
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Food")]
    public static void GenerateFood()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + foodCSVPath);
        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 5)
            {
                return;
            }

            Food foodItem = ScriptableObject.CreateInstance<Food>();
            foodItem.id = int.Parse(splitData[0]);
            foodItem.foodName = splitData[1];
            foodItem.tier = splitData[2];
            foodItem.maxPoints = int.Parse(splitData[3]);
            foodItem.currentPoints = foodItem.maxPoints;
            foodItem.weight = int.Parse(splitData[4]);
            AssetDatabase.CreateAsset(foodItem, $"Assets/Resources/Data/Food/{foodItem.id}.asset");
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/Generate Enemy")]
    public static void GenerateEnemies()
    {
        string[] allLines = File.ReadAllLines(System.IO.Directory.GetCurrentDirectory() + enemyCSVPath);
        foreach (string s in allLines)
        {
            string[] splitData = s.Split(',');

            if (splitData.Length != 6)
            {
                return;
            }

            Enemy enemy = ScriptableObject.CreateInstance<Enemy>();
            enemy.id = int.Parse(splitData[0]);
            enemy.enemyName = splitData[1];
            enemy.walkSpeed = float.Parse(splitData[2]);
            enemy.runSpeed = float.Parse(splitData[3]);
            enemy.lineOfSight = float.Parse(splitData[4]);
            enemy.lineOfSight = float.Parse(splitData[5]);
            AssetDatabase.CreateAsset(enemy, $"Assets/Resources/Data/Enemy/{enemy.id}.asset");
        }
        AssetDatabase.SaveAssets();
    }
}
