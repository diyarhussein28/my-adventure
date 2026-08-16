using UnityEngine;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Manages state transitions and delegates to the active PlayerState.
    /// Implements a Hierarchical Finite State Machine.
    /// </summary>
    public class PlayerStateMachine
    {
        private PlayerController player;
        private PlayerState currentState;
        public PlayerState CurrentState => currentState;

        // State instances (created once, reused)
        public PlayerLocomotionState LocomotionState { get; private set; }
        public PlayerAirborneState AirborneState { get; private set; }
        public PlayerAttackingState AttackingState { get; private set; }
        public PlayerStunnedState StunnedState { get; private set; }
        public PlayerBlockingState BlockingState { get; private set; }
        public PlayerDashingState DashingState { get; private set; }
        public PlayerWallRunState WallRunState { get; private set; }

        public PlayerStateMachine(PlayerController player)
        {
            this.player = player;
            LocomotionState = new PlayerLocomotionState(player, this);
            AirborneState = new PlayerAirborneState(player, this);
            AttackingState = new PlayerAttackingState(player, this);
            StunnedState = new PlayerStunnedState(player, this);
            BlockingState = new PlayerBlockingState(player, this);
            DashingState = new PlayerDashingState(player, this);
            WallRunState = new PlayerWallRunState(player, this);
        }

        /// <summary>
        /// Transition to a new state. Handles Exit/Enter calls.
        /// </summary>
        public void ChangeState(PlayerState newState)
        {
            if (currentState == newState) return;

            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }
    }
}
