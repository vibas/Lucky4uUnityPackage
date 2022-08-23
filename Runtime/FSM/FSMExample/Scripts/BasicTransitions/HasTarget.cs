using FiniteStateMachine;

namespace FSMExample
{
    public class HasTarget : Transition
    {
        private readonly AreaAwareness awareness;

        public HasTarget(State state, AreaAwareness awareness) : base(state)
        {
            this.awareness = awareness;
        }

        public override bool IsValid() => awareness.HasTarget();

        public override void OnTransition() { }

        public override string ToString()
        {
            return "Has Target";
        }
    }
}
