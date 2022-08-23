using FiniteStateMachine;
using UnityEngine;

namespace FSMExample
{
    public class Idle : State
    {
        public Idle(Fsm fsm) : base(fsm)
        {
        }

        public override void OnEnter()
        {
            Debug.LogError("IDLE");
        }

        public override void OnExit()
        {
        }

        protected override void Execute()
        {
        }
    }
}

