using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main menu — title, high score, tap to start.
/// Uses legacy UI.Text for CI compatibility (no TMP import needed).
/// </summary>
public class MainMenu : MonoBehaviour
{
    public Text highScoreText;
    public Text startText;

    private float pulseTime;

    private void Start()
    {
        if (highScoreText != null)
        {
            int hs = PlayerPrefs.GetInt("PetalDrift_HighScore", 0);
            highScoreText.text = "Best: " + hs;
        }
    }

    private void Update()
    {
        pulseTime += Time.deltaTime;
        if (startText != null)
        {
            float alpha = 0.5f + 0.5f * Mathf.Sin(pulseTime * 2f);
            Color c = startText.color;
            c.a = alpha;
            startText.color = c;
        }

        // Tap/click to start
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
        SceneManager.LoadScene("Game");
    }
}
