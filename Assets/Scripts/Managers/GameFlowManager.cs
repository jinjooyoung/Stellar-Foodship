using TMPro;
using UnityEngine;

// 스테이지 선택, 시작, 레벨매니저에서 받아와서 스테이지 세팅, 스테이지 시작 후 3초 카운트 다운 등의 기능을 담당하는 스크립트
// 지금은 스테이지 타이머 시작 로직만 해둠
public class GameFlowManager : MonoBehaviour
{
    [Header("스테이지 타이머")]
    public Timer timer;
    public TextMeshProUGUI timerText;

    void Start()
    {
        timer.StartTimer(LevelManager.Instance.stageTimeLimit);
    }

    private void Update()
    {
        int minutes = Mathf.FloorToInt(timer.CurrentTime / 60f);
        int seconds = Mathf.FloorToInt(timer.CurrentTime % 60f);

        timerText.text = $"{minutes}:{seconds:D2}";
    }
}
