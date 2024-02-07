using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Assets/New Player")]
public class Player : ScriptableObject
{
    public int id;
    public float baseMovementSpeed;
    public int inventoryWeight;
}
