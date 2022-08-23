using UnityEngine;

namespace FiniteStateMachine
{
    public abstract class Fsm : MonoBehaviour
    {
        public bool IsSetUpDone = false;
        private State currentState;

        // Call it from the game object that is using this FSM
        // Once game object is initialized
        public abstract void SetupStates();

        // Call it from the game object that is using this FSM
        // from update method
        public void RunUpdate()
        {
            if(IsSetUpDone)
                currentState.Update();
        }

        protected void SetupFirstState(State state)
        {
            currentState = state;
            currentState.OnEnter();
        }

        public void ChangeState(State newState)
        {
            if (!currentState.CanStateBeChanged()) return;

            currentState.OnExit();
            newState.OnEnter();
            currentState = newState;
        }
    }
}
