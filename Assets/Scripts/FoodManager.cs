using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    #region Variables
    // Food Item Components
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private NoiseController _noiseController;
    public Food _data;

    // Default Data
    public int id;
    public string foodName;
    public string tier;
    public int maxPoints;
    public int currentPoints;
    public int weight;

    // Dynamic Data
    public bool isThrown = false;
    public bool isNearPlayer = false;

    public Food testData;
    #endregion

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _noiseController = transform.Find("Noise").GetComponent<NoiseController>();

        if (testData) SetDefaultFoodData(testData);
    }

    // Used only when spawning food from throwing to set their current points
    // In Throwing script instantiate then call UpdateFoodData
    public void SetFoodData(Food data)
    {
        this.id = data.id;
        this.foodName = data.foodName;
        this.tier = data.tier;
        this.maxPoints = data.maxPoints;
        this.currentPoints = data.currentPoints;
        this.weight = data.weight;
        _data = data;
        // Update Food Sprite
    }

    // Used only when spawning food at start of level since SO saves current points
    // In spawner script instantiate then call SetDefaultFoodData
    private void SetDefaultFoodData(Food data)
    {
        this.id = data.id;
        this.foodName = data.foodName;
        this.tier = data.tier;
        this.maxPoints = data.maxPoints;
        this.currentPoints = this.maxPoints;
        this.weight = data.weight;
        _data = data;
        // Update Food Sprite
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (!isThrown) return;

            StartCoroutine(_noiseController.ProduceNoiseOnce());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isNearPlayer)
        {
            isNearPlayer = true;
            if (!PlayerManager.instance.nearbyFood.Contains(this.gameObject)) PlayerManager.instance.nearbyFood.Add(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isNearPlayer)
        {
            isNearPlayer = false;
            if (PlayerManager.instance.nearbyFood.Contains(this.gameObject)) PlayerManager.instance.nearbyFood.Remove(this.gameObject);
        }
    }
}
