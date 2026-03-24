using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Cooking : MonoBehaviour
{
    [Header("Settings")]
    public float maxTime = 60f;
    [SerializeField] private float currentTime; 
    public bool isCountdown = true;
    public bool runOnStart = false;

    [Header("UI Reference")]
    public Image timerFillImage;
    public TextMeshProUGUI timerText;

    [Header("Events")]
    public UnityEvent onTimerEnd;

    private bool isRunning = false;

    void Start()
    {
        ResetTimer();
        if (runOnStart) isRunning = true;
    }

    void Update()
    {
        if (!isRunning) return;

        // 시간 계산 로직
        if (isCountdown)
        {
            currentTime -= Time.deltaTime; 
            if (currentTime <= 0)
            {
                currentTime = 0;
                FinishTimer();
            }
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime >= maxTime)
            {
                currentTime = maxTime;
                FinishTimer();
            }
        }

        UpdateUI();
    }

    void FinishTimer()
    {
        isRunning = false;
        onTimerEnd?.Invoke();
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;

    public void ResetTimer()
    {
        currentTime = isCountdown ? maxTime : 0f;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timerFillImage != null)
            timerFillImage.fillAmount = currentTime / maxTime;

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
