using UnityEngine;

/// <summary>
/// A glowing flower floating upward. Land on it to score and bounce.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Flower : MonoBehaviour
{
    public float floatSpeed = 40f;
    public float lifetime = 8f;
    public Color[] petalColors = new Color[]
    {
        new Color(1f, 0.4f, 0.7f),    // Pink
        new Color(1f, 0.6f, 0.2f),    // Orange
        new Color(1f, 0.9f, 0.3f),    // Yellow
        new Color(0.9f, 0.95f, 1f),   // White
        new Color(0.3f, 0.85f, 1f),   // Cyan
    };

    private SpriteRenderer spriteRenderer;
    private float age;

    private void Start()
    {
        tag = "Flower";
        gameObject.layer = LayerMask.NameToLayer("Default");

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        float speed = floatSpeed + Random.Range(-10f, 10f);
        float size = 0.7f + Random.Range(-0.15f, 0.15f);
        Color color = petalColors[Random.Range(0, petalColors.Length)];

        transform.localScale = Vector3.one * size;
        if (spriteRenderer != null)
            spriteRenderer.color = color;

        // Slight random rotation
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // Float upward
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, speed);

        // Despawn after lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Gentle sway
        Vector3 pos = transform.position;
        pos.x += Mathf.Sin(Time.time * 1.5f + pos.y * 0.01f) * 0.3f * Time.deltaTime * 60f;
        transform.position = pos;

        // Fade out near end of life
        age += Time.deltaTime;
        float remaining = lifetime - age;
        if (remaining < 2f && remaining > 0)
        {
            float fade = remaining / 2f;
            Color c = spriteRenderer != null ? spriteRenderer.color : Color.white;
            c.a = fade;
            if (spriteRenderer != null) spriteRenderer.color = c;
        }

        // Slow rotation
        transform.Rotate(0, 0, Time.deltaTime * 18f);
    }
}
