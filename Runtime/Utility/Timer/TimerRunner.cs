namespace Lucky4u.Utility
{
    public class TimerRunner : ITimerRunner
    {
        private Timer waitTimer;

        public TimerRunner(float waitTime)
        {
            CreateNewWaitTimer(waitTime);
        }
        public void CreateNewWaitTimer(float waitTime)
        {
            if (waitTimer == null)
                waitTimer = new Timer(waitTime);
        }

        public void RunUpdate()
        {
            if (waitTimer != null)
            {
                waitTimer.RunTimer();
            }
        }

        public bool IsWaitTimerCompleted()
        {
            bool isWaitTimerComplete = false;
            if (waitTimer != null)
                isWaitTimerComplete = waitTimer.IsTimerComplete;
            return isWaitTimerComplete;
        }

        public void ResetWaitTimer()
        {
            if (waitTimer != null)
                waitTimer.Reset();
        }

        public void DestroyWaitTimer()
        {
            waitTimer = null;
        }

        public float GetRemainingTime()
        {
            float remainingTime = 0;
            if (waitTimer != null)
                remainingTime = waitTimer.RemainingTime;
            return remainingTime;
        }
    }
}