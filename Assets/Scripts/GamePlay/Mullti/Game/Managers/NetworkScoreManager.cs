using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class NetworkScoreManager : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI scoreText;

    //------------------------------------------------

    [Networked]
    public int Score { get; set; }

    private int prevScore = -1;

    //------------------------------------------------

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Score = 0;
        }
    }

    public override void Render()
    {
        if (prevScore == Score)
            return;

        prevScore = Score;

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

    void OnEnable()
    {
        Debug.Log("ScoreManager Enable");
    }

    void OnDisable()
    {
        Debug.Log("ScoreManager Disable");
    }
}