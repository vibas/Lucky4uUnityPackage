using UnityEngine;
using System;

namespace FiniteStateMachine
{
    public abstract class State
    {
        private StateCoolDownTimer StateCoolDownTimer;
        protected Fsm fsm;
        private Transition[] transitions;
        public Action<int> OnStateEnterEvent;

        public State(Fsm fsm)
        {
            this.fsm = fsm;
        }

        public void SetTransitions(params Transition[] transitions)
            => this.transitions = transitions;

        public void Update()
        {
            Execute();

            if (StateCoolDownTimer != null)
            {
                StateCoolDownTimer.RunTimer();
            }

            if (CanStateBeChanged())
            {
                CheckTransitions();
            }
        }

        public abstract void OnEnter();
        protected abstract void Execute();
        public abstract void OnExit();

        private void CheckTransitions()
        {
            if (transitions == null) return;
            foreach (var transition in transitions)
            {
                if (transition.IsValid())
                {
                    transition.OnTransition();
                    fsm.ChangeState(transition.GetNextState());
                }
            }
        }

        public virtual bool CanStateBeChanged() => true;

        // ===================== COOL DOWN TIME ======================
        public void CreateNewCoolDownTimer(float waitTime)
        {
            if(StateCoolDownTimer == null)
                StateCoolDownTimer = new StateCoolDownTimer(waitTime);
        }

        public bool IsCoolDownTimerCompleted()
        {
            bool timerComplete = false;
            if(StateCoolDownTimer!=null)
            {
                timerComplete = StateCoolDownTimer.IsCoolDownTimerComplete;
            }
           return timerComplete;
        }

        public void ResetCoolDownTimer()
        {
            if (StateCoolDownTimer != null)
            {
                StateCoolDownTimer.Reset();
            }
        }
        public void DestroyCoolDownTimer()
        {
            StateCoolDownTimer = null;
        }
        // ============================================================
    }

    public class StateCoolDownTimer
    {
        public bool ShouldRunCoolDownTimer;
        public bool IsCoolDownTimerComplete;
        public float CoolDownTime;
        public float CurrentTime;

        public StateCoolDownTimer(float totalCoolDownTime)
        {
            CoolDownTime = totalCoolDownTime;
            CurrentTime = 0;
            ShouldRunCoolDownTimer = true;
        }

        public void RunTimer()
        {
            if (ShouldRunCoolDownTimer)
            {
                if (CurrentTime < CoolDownTime)
                {
                    CurrentTime += Time.deltaTime;
                }
                else
                {
                    OnTimerComplete();
                }
            }
        }

        void OnTimerComplete()
        {
            IsCoolDownTimerComplete = true;
            ShouldRunCoolDownTimer = false;
        }

        public void Reset()
        {
            CurrentTime = 0;
            IsCoolDownTimerComplete = false;
            ShouldRunCoolDownTimer = true;
        }
    }
}
