using FiniteStateMachine;
using UnityEngine;

namespace FSMExample
{
    public class EnemyFsm : Fsm
    {
        [SerializeField] private AreaAwareness areaAwareness;
        [SerializeField] private PatrolData patrolData;
        [SerializeField] private float distanceToStopFollowing;

        public override void SetupStates()
        {
            // ============ INIT STATES ==============//
            State idle = new Idle(this);
            State patrol = new Patrol(this, patrolData);
            State followTarget = new FollowTarget(this,areaAwareness);

            // =========== INIT TRANSITIONS ===========//
            Transition idleToPatrol = new WaitForCoolDown(idle, patrol,3);
            Transition idleToFollow = new HasTarget(followTarget, areaAwareness);
            Transition patrolToIdle = new WaitForCoolDown(patrol,idle,5);
            Transition patrolToFollow = new HasTarget(followTarget, areaAwareness);
            Transition followToPatrol = new TargetIsFar(patrol, distanceToStopFollowing, areaAwareness);

            // =========== ASSIGN TRANSITIONS TO THE STATE ===========//
            idle.SetTransitions(idleToPatrol,idleToFollow);
            patrol.SetTransitions(patrolToIdle,patrolToFollow);
            followTarget.SetTransitions(followToPatrol);

            // ========== SETUP INITIAL STATE ===============//
            SetupFirstState(idle);
        }
    }
}