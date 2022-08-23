using FiniteStateMachine;

namespace FSMExample
{
    public class FollowTarget : State
    {
        AreaAwareness awareness;
        CharacterBehaviour objectBehaviour => fsm.GetComponent<CharacterBehaviour>();

        private int lastDirection;
        private bool stopped = false;

        public FollowTarget(Fsm fsm, AreaAwareness awareness) : base(fsm)
        {
            this.awareness = awareness;
        }

        protected override void Execute()
        {
            int targetDirection = awareness.GetTargetDirection();

            if (targetDirection != lastDirection || stopped)
            {
                stopped = false;
                objectBehaviour.SetXInput(targetDirection);
            }

            lastDirection = targetDirection;
        }

        public override void OnEnter()
        {
            UnityEngine.Debug.LogError("FOLLOW TARGET");

            stopped = true;
            lastDirection = 0;
        }

        public override void OnExit()
        {
            awareness.ResetTarget();
            objectBehaviour.SetXInput(0);
        }

        public override string ToString()
        {
            return "Follow Target";
        }
    }
}
