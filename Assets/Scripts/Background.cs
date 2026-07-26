using UnityEngine;

/// <summary>
/// Scrolling starfield background with parallax layers.
/// Procedurally generated stars rendered as screen-space particles.
/// </summary>
public class Background : MonoBehaviour
{
    public float scrollSpeed = 30f;
    public int starsLayer1Count = 60;
    public int starsLayer2Count = 40;
    public int starsLayer3Count = 20;

    private Camera cam;
    private float screenHalfWidth;
    private float screenHeight;

    // Star data for each layer
    private Star[] stars1, stars2, stars3;
    private Texture2D starTexture;

    private struct Star
    {
        public Vector2 pos;
        public float radius;
        public float brightness;
        public float twinkleSpeed;
        public float twinkleOffset;
    }

    private void Start()
    {
        cam = Camera.main;
        screenHalfWidth = cam.orthographicSize * cam.aspect;
        screenHeight = cam.orthographicSize * 2f;

        stars1 = GenerateStars(starsLayer1Count, 0.3f, 0.5f);
        stars2 = GenerateStars(starsLayer2Count, 0.6f, 0.8f);
        stars3 = GenerateStars(starsLayer3Count, 1.0f, 1.5f);

        // Create a tiny white texture for star rendering
        starTexture = new Texture2D(1, 1);
        starTexture.SetPixel(0, 0, Color.white);
        starTexture.Apply();
    }

    private void Update()
    {
        ScrollStars(stars1, scrollSpeed * 0.3f);
        ScrollStars(stars2, scrollSpeed * 0.7f);
        ScrollStars(stars3, scrollSpeed * 1.2f);
    }

    private Star[] GenerateStars(int count, float minBright, float maxBright)
    {
        Star[] stars = new Star[count];
        float halfH = screenHeight * 0.5f;
        for (int i = 0; i < count; i++)
        {
            stars[i] = new Star
            {
                pos = new Vector2(Random.Range(-screenHalfWidth, screenHalfWidth), Random.Range(-halfH, halfH)),
                radius = Random.Range(0.5f, 2.5f),
                brightness = Random.Range(minBright, maxBright),
                twinkleSpeed = Random.Range(1f, 4f),
                twinkleOffset = Random.Range(0f, Mathf.PI * 2f)
            };
        }
        return stars;
    }

    private void ScrollStars(Star[] stars, float speed)
    {
        float halfH = screenHeight * 0.5f;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].pos.y -= speed * Time.deltaTime;
            if (stars[i].pos.y < -halfH - 1f)
            {
                stars[i].pos.y = halfH + 1f;
                stars[i].pos.x = Random.Range(-screenHalfWidth, screenHalfWidth);
            }
        }
    }

    private void OnGUI()
    {
        // Draw background gradient
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        DrawGradient(screenRect, new Color(0.02f, 0.01f, 0.06f), new Color(0.1f, 0.03f, 0.2f));

        // Draw stars as screen-space points
        DrawStarLayer(stars1, 1.0f);
        DrawStarLayer(stars2, 2.0f);
        DrawStarLayer(stars3, 3.0f);
    }

    private void DrawGradient(Rect rect, Color topColor, Color bottomColor)
    {
        // Simple gradient using vertical strips
        Texture2D gradientTex = new Texture2D(1, 2);
        gradientTex.SetPixel(0, 0, bottomColor);
        gradientTex.SetPixel(0, 1, topColor);
        gradientTex.Apply();
        GUI.DrawTexture(rect, gradientTex);
        Destroy(gradientTex, 0.1f);
    }

    private void DrawStarLayer(Star[] stars, float sizeMultiplier)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            float twinkle = Mathf.Sin(Time.time * stars[i].twinkleSpeed + stars[i].twinkleOffset);
            float brightness = stars[i].brightness * (0.7f + 0.3f * twinkle);
            float radius = stars[i].radius * sizeMultiplier * (0.8f + 0.2f * twinkle);

            // Convert world to screen position
            Vector3 worldPos = new Vector3(stars[i].pos.x, stars[i].pos.y, 0);
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0)
            {
                Color starColor = new Color(brightness, brightness * 0.9f, brightness, brightness);

                // Draw star as a small GUI box (works in OnGUI)
                GUI.color = starColor;
                float size = Mathf.Max(1, radius * 2);
                GUI.DrawTexture(new Rect(screenPos.x - size * 0.5f, Screen.height - screenPos.y - size * 0.5f, size, size), starTexture);
                GUI.color = Color.white;
            }
        }
    }
}
