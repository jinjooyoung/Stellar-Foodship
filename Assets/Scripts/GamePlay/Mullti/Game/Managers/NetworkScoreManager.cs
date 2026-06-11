using Fusion;
using TMPro;
using UnityEngine;

public class NetworkScoreManager : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI scoreText;

    //------------------------------------------------

    [Networked]
    public int Score { get; set; }

    //------------------------------------------------

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Score = 0;
        }

        RefreshUI();
    }

    public override void Render()
    {
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
        if (!Object.HasStateAuthority)
            return;

        Score += amount;
    }

    //------------------------------------------------

    public int GetCurrentScore()
    {
        return Score;
    }
}