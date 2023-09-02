using System;

namespace Runes_and_Spells.UtilityClasses;

public class Timer
{
    public float Time { get; private set; }
    private Action Action { get; set; }
    public bool IsRunning { get; private set; }
    private float DefaultStartTime { get; set; }

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

    public void SetDefaultTime(float milliseconds)
    {
        DefaultStartTime = milliseconds / 1000 * 60;
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