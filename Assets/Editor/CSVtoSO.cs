using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVtoSO
{
    private static string playerCSVPath = "/Assets/Editor/CSVs/Player.csv";

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
}
