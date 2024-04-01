using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FoodManager : MonoBehaviour
{
    #region Variables
    // Food Item Components
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private NoiseController _noiseController;
    private ParticleSystem _particle;
    public Food _data;
    private Transform popUpPrefab;

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
    public float throwRange = 0.0f;
    private Vector3 initialPosition;
    public Food testData;
    private bool hasShownDamage = false;
    #endregion

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        popUpPrefab = Resources.Load<RectTransform>("Prefabs/Popup");
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _noiseController = transform.Find("Noise").GetComponent<NoiseController>();
        _particle = transform.Find("Particle").GetComponent<ParticleSystem>();

        if (testData) SetDefaultFoodData(testData);
    }

    private void Update()
    {
        if (isThrown)
        {
            if (Vector3.Distance(transform.position, initialPosition) > throwRange)
            {
                isThrown = false;
                _rigidBody.velocity = Vector3.zero;
                _particle.Play();

                if (!hasShownDamage)
                {
                    hasShownDamage = true;
                    Transform dmgNumber = Instantiate(popUpPrefab, transform.position, Quaternion.identity);
                    dmgNumber.GetComponent<Popup>().SetDamage(-PlayerManager.instance.thrownFoodPointsDeduction);
                }

                StartCoroutine(_noiseController.ProduceNoiseOnce());
            }

            if (_rigidBody.velocity.magnitude < 1.0f)
            {
                isThrown = false;
                _particle.Play();

                if (!hasShownDamage)
                {
                    hasShownDamage = true;
                    Transform dmgNumber = Instantiate(popUpPrefab, transform.position, Quaternion.identity);
                    dmgNumber.GetComponent<Popup>().SetDamage(-PlayerManager.instance.thrownFoodPointsDeduction);
                }

                StartCoroutine(_noiseController.ProduceNoiseOnce());
            }
        }
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
    public void SetDefaultFoodData(Food data)
    {
        this.id = data.id;
        this.foodName = data.foodName;
        this.tier = data.tier;
        this.maxPoints = data.maxPoints;
        data.currentPoints = data.maxPoints;
        this.currentPoints = data.currentPoints;
        this.weight = data.weight;
        _data = data;
        // Update Food Sprite
    }

    public void Throw(float throwRange)
    {
        isThrown = true;
        this.throwRange = throwRange;
        initialPosition = this.transform.position;
        float throwForce = this.throwRange > 5 ? 35.0f : 15.0f;

        _rigidBody.AddForce(PlayerManager.instance.transform.up * throwForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (!isThrown) return;
            _particle.Play();

            if (!hasShownDamage)
            {
                hasShownDamage = true;
                Transform dmgNumber = Instantiate(popUpPrefab, transform.position, Quaternion.identity);
                dmgNumber.GetComponent<Popup>().SetDamage(-PlayerManager.instance.thrownFoodPointsDeduction);
            }

            StartCoroutine(_noiseController.ProduceNoiseOnce());
            // Lose velocity when hit wall
            _rigidBody.velocity *= 0.10f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isNearPlayer)
        {
            isNearPlayer = true;
            if (!PlayerManager.instance.nearbyFood.Contains(this.gameObject)) PlayerManager.instance.nearbyFood.Add(this.gameObject);
            GameController.instance.currentUIManager.UpdateDisplayInteractables();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isNearPlayer)
        {
            isNearPlayer = false;
            if (PlayerManager.instance.nearbyFood.Contains(this.gameObject)) PlayerManager.instance.nearbyFood.Remove(this.gameObject);
            GameController.instance.currentUIManager.UpdateDisplayInteractables();
        }
    }
}
