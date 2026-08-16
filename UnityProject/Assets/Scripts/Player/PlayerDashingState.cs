using UnityEngine;
using System.Collections;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Dashing state: high-speed movement with iframes.
    /// Can be used on ground or in air (air dash limited by maxAirDashes).
    /// </summary>
    public class PlayerDashingState : PlayerState
    {
        private float dashTimer = 0f;
        private Vector3 dashDirection;

        public PlayerDashingState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            dashTimer = 0f;
            player.CanAct = false;
            player.IsInvincible = true;

            // Determine dash direction
            Vector3 moveDir = GetMovementDirection();
            if (moveDir.sqrMagnitude < 0.01f)
                dashDirection = player.transform.forward;
            else
                dashDirection = moveDir;

            player.RotateTowards(dashDirection);
            player.Dash(dashDirection);

            player.Animator?.SetTrigger("Dash");
            player.Animator?.SetBool("IsDashing", true);
        }

        public override void Exit()
        {
            player.IsInvincible = false;
            player.CanAct = true;
            player.Animator?.SetBool("IsDashing", false);
            player.Animator?.ResetTrigger("Dash");
        }

        public override void Update()
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= player.CharacterStats.dashDuration)
            {
                // Dash complete
                if (AnyAttackInput())
                {
                    stateMachine.ChangeState(stateMachine.AttackingState);
                }
                else if (player.IsGrounded)
                {
                    stateMachine.ChangeState(stateMachine.LocomotionState);
                }
                else
                {
                    stateMachine.ChangeState(stateMachine.AirborneState);
                }
            }
        }

        public override void FixedUpdate()
        {
            // Maintain dash velocity
            float dashSpeed = player.CharacterStats.dashDistance / player.CharacterStats.dashDuration;
            Vector3 dashVelocity = dashDirection * dashSpeed;
            dashVelocity.y = player.Velocity.y; // Preserve gravity
            player.SetVelocity(dashVelocity);
        }
    }
}
