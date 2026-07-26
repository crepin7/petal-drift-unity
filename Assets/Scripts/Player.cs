using System;
using UnityEngine;

/// <summary>
/// The player petal — drifts through cosmos steered by touch.
/// Left half drift left, right half drift right.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Player : MonoBehaviour
{
    public event Action<Vector2> OnFlowerCollected;

    [Header("Movement")]
    public float baseGravity = 180f;
    public float driftAccel = 600f;
    public float driftDamp = 5f;
    public float floatSpeed = -120f;
    public float maxFallSpeed = 400f;
    public float bounceSpeed = -350f;

    [Header("Wobble")]
    public float wobbleSpeed = 2f;
    public float wobbleAmount = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 windForce;
    private bool isTouchingLeft;
    private bool isTouchingRight;
    private bool isFloating;
    private float screenHalfWidth;
    private float screenBottom;
    private bool bounceCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb.gravityScale = 0; // We handle gravity manually
        rb.freezeRotation = true;

        Camera cam = Camera.main;
        screenHalfWidth = cam.orthographicSize * cam.aspect;
        screenBottom = -cam.orthographicSize;
        transform.position = new Vector3(0f, screenBottom * 0.7f, 0f);
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        HandleTouchInput();

        // Drift input
        float driftInput = 0f;
        if (isTouchingLeft) driftInput -= 1f;
        if (isTouchingRight) driftInput += 1f;

        // Horizontal movement
        Vector2 vel = rb.velocity;
        vel.x += (driftInput * driftAccel - vel.x) * Time.deltaTime * driftDamp;

        // Vertical: float up when no input, else gravity
        if (isFloating && driftInput == 0f)
        {
            vel.y += (floatSpeed - vel.y) * Time.deltaTime * 3f;
        }
        else
        {
            vel.y += baseGravity * Time.deltaTime;
        }
        vel.y = Mathf.Clamp(vel.y, -500f, maxFallSpeed);

        // Apply wind
        vel += windForce * Time.deltaTime;

        rb.velocity = vel;

        // Screen wrap horizontal
        Vector3 pos = transform.position;
        if (pos.x < -screenHalfWidth - 1f)
            pos.x = screenHalfWidth + 1f;
        else if (pos.x > screenHalfWidth + 1f)
            pos.x = -screenHalfWidth - 1f;
        transform.position = pos;

        // Wobble
        if (spriteRenderer != null)
        {
            float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, wobble * Mathf.Rad2Deg + vel.x * 0.1f);
        }

        // Game over: fell below screen
        if (pos.y < screenBottom - 2f)
        {
            GameManager.Instance.EndGame();
        }

        // Decay wind force
        windForce = Vector2.Lerp(windForce, Vector2.zero, Time.deltaTime * 2f);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                float touchX = touch.position.x;
                float screenW = Screen.width;
                if (touchX < screenW * 0.5f)
                {
                    isTouchingLeft = true;
                    isTouchingRight = false;
                }
                else
                {
                    isTouchingRight = true;
                    isTouchingLeft = false;
                }
                isFloating = false;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouchingLeft = false;
                isTouchingRight = false;
                isFloating = true;
            }
        }
        else
        {
            // Mouse fallback for editor testing
            if (Input.GetMouseButtonDown(0))
            {
                float mouseX = Input.mousePosition.x;
                float screenW = Screen.width;
                if (mouseX < screenW * 0.5f)
                {
                    isTouchingLeft = true;
                    isTouchingRight = false;
                }
                else
                {
                    isTouchingRight = true;
                    isTouchingLeft = false;
                }
                isFloating = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isTouchingLeft = false;
                isTouchingRight = false;
                isFloating = true;
            }
        }
    }

    public void Bounce()
    {
        if (bounceCooldown) return;
        bounceCooldown = true;
        Vector2 vel = rb.velocity;
        vel.y = bounceSpeed;
        rb.velocity = vel;

        // Flash white
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            StartCoroutine(ResetColorAfterDelay(0.3f));
        }

        Invoke(nameof(ResetBounceCooldown), 0.3f);
    }

    private System.Collections.IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0.85f, 0.4f, 1f);
    }

    private void ResetBounceCooldown()
    {
        bounceCooldown = false;
    }

    public void ApplyWindForce(Vector2 force)
    {
        windForce += force;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flower"))
        {
            OnFlowerCollected?.Invoke(other.transform.position);
            Bounce();
            GameManager.Instance.AddScore(10);
            GameManager.Instance.IncrementCombo();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Hazard"))
        {
            GameManager.Instance.EndGame();
        }
    }

    // Legacy OnGUI for mouse input fallback
    private void OnGUI()
    {
        if (!GameManager.Instance.IsPlaying) return;
        // Touch input is handled in Update via Input.touchCount
    }
}
