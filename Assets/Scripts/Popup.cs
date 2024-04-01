using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private float destroyTimer = 0.2f;

    private Color textColor;
    private TextMeshPro textMesh;

    private Vector3 RandomizeIntensity = new Vector3(0.5f, 0.5f, 0);

    private void Awake()
    {
        textMesh = gameObject.GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
            Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
            Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z));
    }

    public void SetDamage(float damage)
    {
        textMesh.text = damage.ToString();
        textColor = textMesh.color;
        destroyTimer = 0.2f;
    }

    private void LateUpdate()
    {
        destroyTimer -= Time.deltaTime;
        if (destroyTimer < 0)
        {
            float destroySpeed = 2.5f;
            textColor.a -= destroySpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
