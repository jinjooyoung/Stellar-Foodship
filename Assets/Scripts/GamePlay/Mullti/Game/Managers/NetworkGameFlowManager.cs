using Fusion;
using JinJooYoung;
using TMPro;
using UnityEngine;

public class NetworkGameFlowManager : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private UIPopup overCanvas;

    [SerializeField]
    private TextMeshProUGUI finalScoreText;

    [Header("Reference")]
    [SerializeField]
    private NetworkScoreManager scoreManager;
    private FusionBootstrap bootstrap;

    //------------------------------------------------

    [Networked]
    public float NetCurrentTime { get; set; }

    [Networked]
    public float NetMaxTime { get; set; }

    [Networked]
    public NetworkBool IsGameOver { get; set; }

    [Networked]
    public float StartDelay { get; set; }

    //------------------------------------------------

    private bool prevGameOver = false;

    private NetworkTimer timer = new();

    //------------------------------------------------

    public override void Spawned()
    {
        if (overCanvas != null)
        {
            overCanvas.Initialize();
        }

        if (Object.HasStateAuthority)
        {
            StartDelay = 3f;

            timer.Start(
                LevelManager.Instance.stageTimeLimit);

            timer.Stop();

            SyncTimer();
        }

        bootstrap =
            FindFirstObjectByType<FusionBootstrap>();
    }

    //------------------------------------------------

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsGameOver)
            return;

        if (StartDelay > 0)
        {
            StartDelay -= Runner.DeltaTime;

            if (StartDelay <= 0)
            {
                StartDelay = 0;

                timer.Resume();
            }

            return;
        }

        if (timer.Tick(Runner.DeltaTime))
        {
            EndGame();
        }

        SyncTimer();
    }

    //------------------------------------------------

    public override void Render()
    {
        UpdateTimerUI();

        if (!prevGameOver && IsGameOver)
        {
            prevGameOver = true;

            ShowGameOverUI();
        }
    }

    //------------------------------------------------

    void SyncTimer()
    {
        NetCurrentTime = timer.CurrentTime;
        NetMaxTime = timer.MaxTime;
    }

    //------------------------------------------------

    void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes =
            Mathf.FloorToInt(NetCurrentTime / 60f);

        int seconds =
            Mathf.FloorToInt(NetCurrentTime % 60f);

        timerText.text =
            $"{minutes}:{seconds:D2}";
    }

    //------------------------------------------------

    void EndGame()
    {
        IsGameOver = true;

        DisablePlayers();
    }

    //------------------------------------------------

    void DisablePlayers()
    {
        NewPlayer[] players = FindObjectsByType<NewPlayer>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            player.State =
                PlayerState.Uncontrollable;
        }
    }

    //------------------------------------------------

    void ShowGameOverUI()
    {
        if (overCanvas != null)
        {
            overCanvas.Open();
        }

        if (finalScoreText != null)
        {
            finalScoreText.text =
                $"최종 점수 : {scoreManager.GetCurrentScore()}";
        }
    }

    //------------------------------------------------

    public float GetTimeProgress()
    {
        if (NetMaxTime <= 0)
            return 0f;

        return NetCurrentTime / NetMaxTime;
    }

    //------------------------------------------------

    public void BackToStage()
    {
        if (bootstrap == null)
            return;

        if (bootstrap.Runner == null)
            return;

        if (!bootstrap.Runner.IsServer)
            return;

        bootstrap.StartStageSelectScene();
    }
}