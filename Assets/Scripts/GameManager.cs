using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Global game manager — tracks score, combo, game state.
/// Singleton that persists across scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboUpdated;
    public event Action<int> OnGameOver;
    public event Action OnGameStarted;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int HighScore { get; private set; }
    public bool IsPlaying { get; private set; }

    private const string SaveKey = "PetalDrift_HighScore";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadHighScore();
    }

    public void AddScore(int points)
    {
        Score += points * Mathf.Max(1, Combo);
        OnScoreChanged?.Invoke(Score);
    }

    public void IncrementCombo()
    {
        Combo++;
        OnComboUpdated?.Invoke(Combo);
    }

    public void ResetCombo()
    {
        Combo = 0;
        OnComboUpdated?.Invoke(Combo);
    }

    public void StartGame()
    {
        Score = 0;
        Combo = 0;
        IsPlaying = true;
        OnGameStarted?.Invoke();
    }

    public void EndGame()
    {
        IsPlaying = false;
        if (Score > HighScore)
        {
            HighScore = Score;
            SaveHighScore();
        }
        OnGameOver?.Invoke(Score);
    }

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(SaveKey, 0);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(SaveKey, HighScore);
        PlayerPrefs.Save();
    }
}
