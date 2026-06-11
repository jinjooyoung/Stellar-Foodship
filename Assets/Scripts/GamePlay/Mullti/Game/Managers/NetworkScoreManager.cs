using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class NetworkScoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI scoreText;

    //------------------------------------------------

    [Networked]
    public int Score { get; set; }

    //------------------------------------------------

    void Awake()
    {
        Score = 0;
        RefreshUI();
    }

    //------------------------------------------------

    void RefreshUI()
    {
        if (scoreText == null)
            return;

        scoreText.text = Score.ToString();
    }

    //------------------------------------------------

    public void AddScore(int amount)
    {
        Score += amount;
        RefreshUI();
    }

    //------------------------------------------------

    public int GetCurrentScore()
    {
        return Score;
    }
}