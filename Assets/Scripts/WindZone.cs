using UnityEngine;

/// <summary>
/// Wind current that pushes the petal in a direction.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class WindZone : MonoBehaviour
{
    public enum WindDirection { Left, Right, Up, Down }

    public float windStrength = 100f;
    public WindDirection windDir = WindDirection.Right;
    public float lifetime = 8f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        tag = "WindZone";
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        windStrength = 50f + Random.Range(-20f, 80f);
        windDir = (WindDirection)Random.Range(0, 4);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = true;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0.95f, 0.7f, 0.4f);

        Destroy(gameObject, lifetime);
    }

    public Vector2 GetWindForce()
    {
        switch (windDir)
        {
            case WindDirection.Left:  return new Vector2(-windStrength, 0);
            case WindDirection.Right: return new Vector2(windStrength, 0);
            case WindDirection.Up:    return new Vector2(0, -windStrength);
            case WindDirection.Down:  return new Vector2(0, windStrength);
        }
        return Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
                player.ApplyWindForce(GetWindForce());
        }
    }
}
