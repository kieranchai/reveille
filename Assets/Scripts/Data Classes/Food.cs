using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Assets/New Food")]
public class Food : ScriptableObject
{
    public int id;
    public string foodName;
    public string tier;
    public int maxPoints;
    public int currentPoints;
    public int weight;
}
