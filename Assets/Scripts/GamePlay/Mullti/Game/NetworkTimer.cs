using System;

[Serializable]
public class NetworkTimer
{
    public float MaxTime { get; private set; }

    public float CurrentTime { get; private set; }

    public bool IsRunning { get; private set; }

    public float Normalized => MaxTime <= 0 ? 0 : CurrentTime / MaxTime;

    public event Action OnCompleted;

    // 시작
    public void Start(float time)
    {
        MaxTime = time;
        CurrentTime = time;
        IsRunning = true;
    }

    // 재시작 없이 이어하기
    public void Resume()
    {
        if (CurrentTime > 0f)
            IsRunning = true;
    }

    // 일시정지
    public void Stop()
    {
        IsRunning = false;
    }

    // 남은시간 증가
    public void AddTime(float time)
    {
        CurrentTime += time;

        if (CurrentTime > MaxTime)
            CurrentTime = MaxTime;
    }

    // 남은시간 감소
    public void Tick(float deltaTime)
    {
        if (!IsRunning)
            return;

        CurrentTime -= deltaTime;

        if (CurrentTime <= 0f)
        {
            CurrentTime = 0f;
            IsRunning = false;

            OnCompleted?.Invoke();
        }
    }

    // 남은시간 강제설정
    public void SetCurrentTime(float time)
    {
        CurrentTime = Math.Clamp(time, 0f, MaxTime);
    }

    // 최대시간 변경
    public void SetMaxTime(float time)
    {
        MaxTime = time;

        if (CurrentTime > MaxTime)
            CurrentTime = MaxTime;
    }

    // 초기화
    public void Reset()
    {
        MaxTime = 0f;
        CurrentTime = 0f;
        IsRunning = false;
    }
}