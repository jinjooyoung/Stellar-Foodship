using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [Header("설정 패널")]
    public GameObject settingCanvas;

    [Header("플레이어")]
    public Player[] players;

    bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void StopInputPlayer()
    {
        foreach(Player p in players)
        {
            p.state = PlayerState.Uncontrollable;
        }
    }

    public void StartInputPlayer()
    {
        foreach (Player p in players)
        {
            p.state = PlayerState.Controllable;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            settingCanvas.SetActive(true);
            StopInputPlayer();
        }
        else
        {
            Time.timeScale = 1f;
            settingCanvas.SetActive(false);
            StartInputPlayer();
        }
    }
}
