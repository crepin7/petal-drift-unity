using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scrolling starfield background with parallax layers.
/// Renders via UI Image components (not OnGUI) so Canvas text stays visible.
/// </summary>
public class Background : MonoBehaviour
{
    [Header("Scrolling")]
    public float scrollSpeed = 30f;

    [Header("Stars per layer")]
    public int starsLayer1Count = 60;
    public int starsLayer2Count = 40;
    public int starsLayer3Count = 20;

    private Camera cam;
    private float screenHalfWidth;
    private float screenHeight;

    // Star data for 3 layers
    private Star[] stars1, stars2, stars3;

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

    private void Update()
    {
        ScrollStars(stars1, scrollSpeed * 0.3f);
        ScrollStars(stars2, scrollSpeed * 0.7f);
        ScrollStars(stars3, scrollSpeed * 1.2f);
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

    // Draw gradient and stars using GL — renders before Canvas UI
    private void OnRenderObject()
    {
        if (cam == null) return;

        GL.PushMatrix();
        GL.LoadOrtho();

        // Draw gradient background
        DrawGradient();

        // Draw star layers
        DrawStarLayer(stars1, 1.0f);
        DrawStarLayer(stars2, 2.0f);
        DrawStarLayer(stars3, 3.0f);

        GL.PopMatrix();
    }

    private void DrawGradient()
    {
        Material gradientMat = new Material(Shader.Find("Hidden/Internal-Colored"));
        gradientMat.hideFlags = HideFlags.HideAndDontSave;
        gradientMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        gradientMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        gradientMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        gradientMat.SetInt("_ZWrite", 0);

        gradientMat.SetPass(0);

        Color topColor = new Color(0.1f, 0.03f, 0.2f);
        Color bottomColor = new Color(0.02f, 0.01f, 0.06f);

        GL.Begin(GL.TRIANGLE_STRIP);
        GL.Color(topColor);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(1, 1, 0);
        GL.Color(bottomColor);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.End();
    }

    private void DrawStarLayer(Star[] stars, float sizeMultiplier)
    {
        Material starMat = new Material(Shader.Find("Hidden/Internal-Colored"));
        starMat.hideFlags = HideFlags.HideAndDontSave;
        starMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        starMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        starMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        starMat.SetInt("_ZWrite", 0);

        starMat.SetPass(0);

        GL.Begin(GL.QUADS);

        for (int i = 0; i < stars.Length; i++)
        {
            float twinkle = Mathf.Sin(Time.time * stars[i].twinkleSpeed + stars[i].twinkleOffset);
            float brightness = stars[i].brightness * (0.7f + 0.3f * twinkle);
            float radius = stars[i].radius * sizeMultiplier * (0.8f + 0.2f * twinkle);

            // Convert world position to normalized viewport position
            Vector3 worldPos = new Vector3(stars[i].pos.x, stars[i].pos.y, 0);
            Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);

            if (viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
            {
                float pixelSize = radius * 2f;
                float halfSize = (pixelSize / Screen.height) * 0.5f; // Normalized size

                Color starColor = new Color(brightness, brightness * 0.9f, brightness, brightness);
                GL.Color(starColor);

                float x = viewportPos.x;
                float y = viewportPos.y;
                float h = halfSize;

                // Draw quad
                GL.Vertex3(x - h, y - h, 0);
                GL.Vertex3(x + h, y - h, 0);
                GL.Vertex3(x + h, y + h, 0);
                GL.Vertex3(x - h, y + h, 0);
            }
        }

        GL.End();
    }
}
