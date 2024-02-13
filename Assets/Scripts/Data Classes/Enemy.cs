using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Assets/New Enemy")]
public class Enemy : ScriptableObject
{
    public int id;
    public string enemyName;
    public float walkSpeed;
    public float runSpeed;
    public float lineOfSight;
    public float fieldOfView;
}
