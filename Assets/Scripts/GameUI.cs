using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// In-game UI: score, combo, game over overlay.
/// Uses legacy UI.Text for CI compatibility.
/// </summary>
public class GameUI : MonoBehaviour
{
    public Text scoreText;
    public Text comboText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text highScoreText;
    public Text tapToRestartText;

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
