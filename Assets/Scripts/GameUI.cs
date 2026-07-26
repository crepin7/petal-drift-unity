using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// In-game UI: score, combo, game over overlay.
/// </summary>
public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI tapToRestartText;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        GameManager.Instance.OnScoreChanged += UpdateScore;
        GameManager.Instance.OnComboUpdated += UpdateCombo;
        GameManager.Instance.OnGameOver += ShowGameOver;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnComboUpdated -= UpdateCombo;
            GameManager.Instance.OnGameOver -= ShowGameOver;
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private void UpdateCombo(int combo)
    {
        if (comboText != null)
            comboText.text = combo > 1 ? "x" + combo : "";
    }

    private void ShowGameOver(int score)
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (finalScoreText != null)
            finalScoreText.text = "Score: " + score;
        if (highScoreText != null)
            highScoreText.text = "Best: " + GameManager.Instance.HighScore;
    }

    private void Update()
    {
        // Restart on tap during game over
        if (!GameManager.Instance.IsPlaying && gameOverPanel != null && gameOverPanel.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                GameManager.Instance.StartGame();
                SceneManager.LoadScene("Game");
            }
        }
    }
}
