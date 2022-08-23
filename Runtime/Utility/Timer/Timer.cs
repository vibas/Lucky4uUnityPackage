using UnityEngine;
public class Timer
{
    public bool ShouldRunTimer;
    public bool IsTimerComplete;
    public float waitTime;
    public float currentTime;
    public float RemainingTime => waitTime - currentTime;

    public Timer(float totalWaitTime)
    {
        waitTime = totalWaitTime;
        currentTime = 0;
        ShouldRunTimer = true;
    }

    public void RunTimer()
    {
        if (ShouldRunTimer)
        {
            if (currentTime < waitTime)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                OnTimerComplete();
            }
        }
    }

    void OnTimerComplete()
    {
        IsTimerComplete = true;
        ShouldRunTimer = false;
    }

    public void Reset()
    {
        currentTime = 0;
        IsTimerComplete = false;
        ShouldRunTimer = true;
    }
}