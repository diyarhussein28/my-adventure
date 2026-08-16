using UnityEngine;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Base class for all player states in the Hierarchical State Machine.
    /// Each state handles Enter, Update, FixedUpdate, and Exit logic.
    /// </summary>
    public abstract class PlayerState
    {
        protected PlayerController player;
        protected PlayerStateMachine stateMachine;

        public PlayerState(PlayerController player, PlayerStateMachine stateMachine)
        {
            this.player = player;
            this.stateMachine = stateMachine;
        }

        /// <summary>
        /// Called once when entering this state.
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Called every frame while in this state.
        /// Handle input reading and state transition checks here.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Called every fixed timestep while in this state.
        /// Handle physics here.
        /// </summary>
        public virtual void FixedUpdate() { }

        /// <summary>
        /// Called once when leaving this state.
        /// Cleanup animations, timers, etc.
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// Helper to get smoothed input direction relative to camera.
        /// </summary>
        protected Vector3 GetMovementDirection()
        {
            var input = SeasOfLegends.Input.InputManager.Instance;
            if (input == null) return Vector3.zero;

            Vector3 camForward = player.CameraTransform?.forward ?? Vector3.forward;
            Vector3 camRight = player.CameraTransform?.right ?? Vector3.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            return (camForward * input.MoveInput.y + camRight * input.MoveInput.x).normalized;
        }

        /// <summary>
        /// Check if any attack input was pressed this frame.
        /// </summary>
        protected bool AnyAttackInput()
        {
            var input = SeasOfLegends.Input.InputManager.Instance;
            if (input == null) return false;
            return input.LightAttackPressed || input.HeavyAttackPressed || 
                   input.SpecialAttackPressed || input.GrabPressed || input.UltimatePressed;
        }
    }
}
