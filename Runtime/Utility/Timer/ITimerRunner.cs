namespace Lucky4u.Utility
{
    public interface ITimerRunner
    {
        void CreateNewWaitTimer(float waitTime);
        void RunUpdate();
        bool IsWaitTimerCompleted();
        float GetRemainingTime();
        void ResetWaitTimer();
        void DestroyWaitTimer();
    }
}