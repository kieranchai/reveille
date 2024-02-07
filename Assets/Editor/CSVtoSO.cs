using UnityEditor;
using UnityEngine;
using System.IO;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

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

            if (splitData.Length != 5)
            {
                return;
            }

            Player player = ScriptableObject.CreateInstance<Player>();
            player.id = int.Parse(splitData[0]);
            player.baseMovementSpeed = float.Parse(splitData[1]);
            player.inventoryWeight = int.Parse(splitData[2]);
            AssetDatabase.CreateAsset(player, $"Assets/Resources/Data/Players/{player.id}.asset");
        }
        AssetDatabase.SaveAssets();
    }
}
