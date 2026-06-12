using System;

[Serializable]
public class NetworkTimer
{
    public float MaxTime { get; private set; }

    public float CurrentTime { get; private set; }

    public bool IsRunning { get; private set; }

    public float Normalized =>
        MaxTime == 0 ? 0 : CurrentTime / MaxTime;

    public void Start(float time)
    {
        MaxTime = time;
        CurrentTime = time;
        IsRunning = true;
    }

    public void Resume()
    {
        if (CurrentTime > 0)
            IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void AddTime(float t)
    {
        CurrentTime += t;

        if (CurrentTime > MaxTime)
            CurrentTime = MaxTime;
    }

    public bool Tick(float delta)
    {
        if (!IsRunning)
            return false;

        CurrentTime -= delta;

        if (CurrentTime <= 0)
        {
            CurrentTime = 0;
            IsRunning = false;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        MaxTime = 0;
        CurrentTime = 0;
        IsRunning = false;
    }

    // 클래스로 만들었는데 sturct로 바꿨다가 혹시나 생길 문제를 방지하기 위해서 타이머 배열 당길때 참조형이라서 그냥 카피하는 함수 하나 씀
    public void CopyFrom(NetworkTimer other)
    {
        MaxTime = other.MaxTime;
        CurrentTime = other.CurrentTime;
        IsRunning = other.IsRunning;
    }
}