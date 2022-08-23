using FiniteStateMachine;

namespace FSMExample
{
    public class TargetIsClose : Transition
    {
        private readonly float distance;
        private readonly AreaAwareness awareness;

        public TargetIsClose(State state, float distance, AreaAwareness awareness) : base(state)
        {
            this.distance = distance;
            this.awareness = awareness;
        }

        public override bool IsValid() => awareness.GetTargetDistance() < distance;

        public override void OnTransition() { }

        public override string ToString()
        {
            return "Target is Close";
        }
    }
}
