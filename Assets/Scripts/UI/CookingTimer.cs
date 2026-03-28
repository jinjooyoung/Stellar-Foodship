using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Events; 
using TMPro;

public class CookingTimer : MonoBehaviour
{
    public enum TimerType { Stage, Tool } // 스테이지용인지 도구용인지 구분

    [Header("설정")]
    public TimerType timerType = TimerType.Tool;
    public float maxTime = 180f; 
    private float currentTime;
    private bool isRunning = false;
    private bool isCompleted = false;

    [Header("UI 연결")]
    public TextMeshProUGUI timerText; 
    public Image progressBar;        // 게이지 표시

    [Header("이벤트")]
    public UnityEvent onTimerFinished; 

    void Start()
    {
        ResetTimer();

       
        if (timerType == TimerType.Stage)
        {
            isRunning = true;
        }
    }

    void Update()
    {
        if (!isRunning || isCompleted) return;

        
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();
        }
        else
        {
            currentTime = 0;
            UpdateUI();
            Finish();
        }
    }


    // 재료를 넣었을 때 호출
    public void StartCooking()
    {
        if (isCompleted) return;
        isRunning = true;
    }

    // 재료를 뺐을 때 호출 (멈춤)
    public void StopCooking()
    {
        isRunning = false;
    }

    // 초기화 (음식을 접시에 담았을 때 등)
    public void ResetTimer()
    {
        currentTime = maxTime;
        isCompleted = false;
        isRunning = (timerType == TimerType.Stage); // 스테이지면 다시 작동
        UpdateUI();
    }



    void UpdateUI()
    {
        // 1. 텍스트 업데이트 (00:00 형식)
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // 2. 게이지 업데이트 (0~1 사이 값)
        if (progressBar != null)
        {
            // 남은 시간에 비례해서 게이지가 깎임 (혹은 1 - (cur/max) 로 하면 차오름)
            progressBar.fillAmount = currentTime / maxTime;
        }
    }

    void Finish()
    {
        isRunning = false;
        isCompleted = true;

        if (timerType == TimerType.Stage)
        {
            Debug.Log("스테이지 종료!");
        }
        else
        {
            Debug.Log("요리 완료!");
        }

        
        onTimerFinished?.Invoke();
    }
}