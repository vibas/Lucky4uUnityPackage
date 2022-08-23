using FiniteStateMachine;

namespace FSMExample
{
    public class WaitForCoolDown : Transition
    {
        State currentState;
        public WaitForCoolDown(State currentState, State targetState, float waitTime) : base(targetState)
        {
            this.currentState = currentState;
            this.currentState.CreateNewCoolDownTimer(waitTime);
        }

        public override bool IsValid()
        {
            bool isValid = currentState.IsCoolDownTimerCompleted();
            if (isValid)
            {
                currentState.ResetCoolDownTimer();
            }
            return isValid;
        }

        public override void OnTransition()
        {
            
        }
    }
}

