using System.Runtime.InteropServices;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("현재 점수")]
    public int score;

    [Header("콤보 점수")]
    public int[] comboScore = { 10, 15, 20, 25, 30 };

    void Awake()
    {
        score = 0;
    }

    public void AddScore(int score)
    {
        this.score += score;
    }

    public int GetComboBonus(int combo)
    {
        if (combo < 0 || combo > 5) return 0;

        return comboScore[combo - 1];
    }

    public int GetCurrentScore()
    {
        return score;
    }
}
