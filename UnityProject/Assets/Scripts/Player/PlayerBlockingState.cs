using UnityEngine;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Blocking state: reduces incoming damage, can parry with precise timing.
    /// Transitions to: Locomotion (release block), Attacking (attack input).
    /// </summary>
    public class PlayerBlockingState : PlayerState
    {
        private float blockStartTime;
        private const float PARRY_WINDOW = 0.15f; // Seconds for perfect parry

        public PlayerBlockingState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            player.IsBlocking = true;
            player.CanAct = false;
            blockStartTime = Time.time;
            player.Animator?.SetBool("IsBlocking", true);
            player.Animator?.SetTrigger("BlockStart");
        }

        public override void Exit()
        {
            player.IsBlocking = false;
            player.CanAct = true;
            player.Animator?.SetBool("IsBlocking", false);
            player.Animator?.ResetTrigger("BlockStart");
        }

        public override void Update()
        {
            var input = SeasOfLegends.Input.InputManager.Instance;
            if (input == null) return;

            if (!input.BlockPressed)
            {
                stateMachine.ChangeState(player.IsGrounded ? stateMachine.LocomotionState : stateMachine.AirborneState);
                return;
            }

            if (AnyAttackInput())
            {
                // Can attack out of block (slow)
                stateMachine.ChangeState(stateMachine.AttackingState);
                return;
            }

            if (input.DashPressed)
            {
                // Dodge while blocking
                stateMachine.ChangeState(stateMachine.DashingState);
            }
        }

        public override void FixedUpdate()
        {
            // Slower movement while blocking
            Vector3 moveDir = GetMovementDirection();
            player.RotateTowards(moveDir);
            player.Move(moveDir, 0.4f); // 40% speed while blocking
        }

        /// <summary>
        /// Checks if the player is within the parry window.
        /// Called by CombatSystem when an attack connects.
        /// </summary>
        public bool IsInParryWindow()
        {
            return Time.time - blockStartTime <= PARRY_WINDOW;
        }
    }
}
