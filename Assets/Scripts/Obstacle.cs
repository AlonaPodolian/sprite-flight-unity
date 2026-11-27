using NUnit.Framework.Internal;
using UnityEngine;


public class Obstacle : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 2.0f;
    
    public float minSpeed = 50f;
    public float maxSpeed = 150f;
    public float maxSpinSpeed = 10f;
    public GameObject bounceEffectPrefab;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        rb = GetComponent<Rigidbody2D>();

        float difficultyMultiplier = 1f + Time.timeSinceLevelLoad * 0.005f;

        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;
        randomSpeed *= difficultyMultiplier;

        Vector2 randomDirection = Random.insideUnitCircle;
        rb.AddForce(randomDirection * randomSpeed);

        float currentMaxSpeed = maxSpeed * difficultyMultiplier;

        if (rb.linearVelocity.magnitude > currentMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentMaxSpeed;
        }

        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb == null || rb.bodyType == RigidbodyType2D.Static) return;

        rb.linearVelocity *= 1f + Time.fixedDeltaTime * 0.05f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;

        GameObject bounceEffect = Instantiate(
            bounceEffectPrefab,
            contactPoint,
            Quaternion.identity
        );

        Destroy(bounceEffect, 1f);
    }
}
