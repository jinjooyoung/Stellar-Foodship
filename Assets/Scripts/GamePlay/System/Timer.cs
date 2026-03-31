using System;
using UnityEngine;
using UnityEngine.UI;

// 타이머 필요한 오브젝트에 컴포넌트로 붙이지 않고 public Timer timer = new Timer(); 로 필요한 스크립트에서 생성해서 사용
public class Timer : MonoBehaviour
{
    // 외부에서 읽기만 가능
    public float MaxTime { get; private set; }
    public float CurrentTime { get; private set; }
    public bool IsRunning { get; private set; }

    public float Normalized => MaxTime > 0f ? CurrentTime / MaxTime : 0f;

    // 외부에서 등록 및 해제만 가능. Invoke 불가능
    public event Action OnCompleted;

    public GameObject timerSlider;
    UnityEngine.UI.Slider slider;

    void Awake()
    {
        timerSlider.SetActive(false);
        slider = timerSlider.GetComponent<UnityEngine.UI.Slider>();
    }

    void Update()
    {
        Tick(Time.deltaTime);
    }

    // 타이머 시작 및 초기화
    public void StartTimer(float time)
    {
        MaxTime = time;
        slider.maxValue = 1f;
        slider.value = 0f;
        CurrentTime = time;
        IsRunning = true;
        timerSlider.SetActive(true);
    }

    // 타이머 정지
    public void Stop()
    {
        IsRunning = false;
    }

    // 타이머 재개
    public void Resume()
    {
        if (CurrentTime > 0f)
        {
            IsRunning = true;
        }
    }

    // 남은 시간 증가
    public void AddTime(float time)
    {
        CurrentTime += time;

        if (CurrentTime >= MaxTime)
        {
            CurrentTime = MaxTime;
        }
    }

    // 시간 감소 처리
    public void Tick(float deltaTime)
    {
        if (!IsRunning) return;

        CurrentTime -= deltaTime;
        slider.value = 1 - Normalized;

        if (CurrentTime <= 0f)
        {
            CurrentTime = 0f;
            IsRunning = false;
            timerSlider.SetActive(false);
            OnCompleted?.Invoke();
        }
    }
}