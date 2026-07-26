using UnityEngine;

/// <summary>
/// Dark cloud that pushes the petal downward.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Hazard : MonoBehaviour
{
    public float lifetime = 10f;

    private SpriteRenderer spriteRenderer;
    private Vector2 driftSpeed;

    private void Start()
    {
        tag = "Hazard";
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        driftSpeed = new Vector2(
            Random.Range(-15f, 15f),
            Random.Range(-10f, -5f)
        );

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = driftSpeed;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.2f, 0.1f, 0.35f, 0.7f);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Gentle pulse
        float pulse = 1f + Mathf.Sin(Time.time * 2f) * 0.05f;
        transform.localScale = new Vector3(1.5f * pulse, 0.8f * pulse, 1f);
    }
}
