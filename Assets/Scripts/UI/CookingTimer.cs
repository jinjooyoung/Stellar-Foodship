using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Events; 
using TMPro;

public class CookingTimer : MonoBehaviour
{
    public enum TimerType { Stage, Tool } // 스테이지용인지 도구용인지 구분

    [Header("설정")]
    public TimerType timerType = TimerType.Tool;
    public float maxTime = 0.5f; 
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


   
    public void StartCooking()
    {
        if (isCompleted) return;
        isRunning = true;
    }

    
    public void StopCooking()
    {
        isRunning = false;
    }

   
    public void ResetTimer()
    {
        currentTime = maxTime;
        isCompleted = false;
        isRunning = (timerType == TimerType.Stage); // 스테이지면 다시 작동
        UpdateUI();
    }



    void UpdateUI()
    {
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

       
        if (progressBar != null)
        {
            
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