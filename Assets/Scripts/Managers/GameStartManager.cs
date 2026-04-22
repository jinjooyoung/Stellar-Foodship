using UnityEngine;
using TMPro;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI countdownText;

    [Header("플레이어 설정 (2명 드래그)")]
    public Player[] players; 

    [Header("타이머 스크립트")]
    public GameFlowManager gameFlowManager; 

    void Start()
    {
        // 1. 시작하자마자 모든 플레이어 조작 비활성화
        foreach (Player p in players)
        {
            if (p == null) continue;

            p.state = PlayerState.Uncontrollable;
        }

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        float timer = 3f;
        while (timer > 0)
        {
            countdownText.text = Mathf.Ceil(timer).ToString();
            yield return null;
            timer -= Time.deltaTime;
        }

        countdownText.text = "GO!";

        
        foreach (Player p in players)
        {
            if (p == null) continue;

            p.state = PlayerState.Controllable;
        }

        // 4. 스테이지 타이머 시작
        gameFlowManager.timer.StartTimer(LevelManager.Instance.stageTimeLimit);

        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }
}