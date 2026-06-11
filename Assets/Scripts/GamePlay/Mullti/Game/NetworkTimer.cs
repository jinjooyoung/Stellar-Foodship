using System;

[Serializable]
public class NetworkTimer
{
    public float MaxTime { get; private set; }

    public float CurrentTime { get; private set; }

    public bool IsRunning { get; private set; }

    public float Normalized =>
        MaxTime == 0 ? 0 : CurrentTime / MaxTime;

    public event Action OnCompleted;

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
            OnCompleted?.Invoke();
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
}