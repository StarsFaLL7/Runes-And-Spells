using System;

namespace Runes_and_Spells;

public class Timer
{
    public float Time { get; private set; }
    public Action Action { get; private set; }
    public bool IsRunning { get; private set; }
    public float DefaultStartTime { get; private set; }

    public Timer(float defaultMilliseconds, Action actionOnEnd)
    {
        Action = actionOnEnd;
        DefaultStartTime = defaultMilliseconds / 1000 * 60;
        Time = defaultMilliseconds / 1000 * 60;
    }

    public void Tick()
    {
        if (!IsRunning) return;
        
        Time -= 1;
        if (Action is not null && Time <= 0)
        {
            Stop();
            Action();
        }
    }

    public void SetTime(float milliseconds)
    {
        Time = milliseconds / 1000 * 60;
    }

    public void StartWithTime(float milliseconds)
    {
        Time = milliseconds / 1000 * 60;
        IsRunning = true;
    }

    public void StartAgain()
    {
        Time = DefaultStartTime;
        IsRunning = true;
    }
    
    public void Start() => IsRunning = true;
    public void Stop() => IsRunning = false;

}