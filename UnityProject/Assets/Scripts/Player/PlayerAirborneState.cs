using UnityEngine;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Airborne state: jumping, falling, aerial attacks, air dashes.
    /// Transitions to: Locomotion (land), WallRun (touch wall), Attacking (attack input), Dashing (dash input).
    /// </summary>
    public class PlayerAirborneState : PlayerState
    {
        private bool hasJumped;

        public PlayerAirborneState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            player.Animator?.SetBool("IsAirborne", true);
            hasJumped = true;
        }

        public override void Exit()
        {
            player.Animator?.SetBool("IsAirborne", false);
        }

        public override void Update()
        {
            var input = SeasOfLegends.Input.InputManager.Instance;
            if (input == null) return;

            // Priority: Attack > Dash > WallRun > Land

            if (AnyAttackInput())
            {
                stateMachine.ChangeState(stateMachine.AttackingState);
                return;
            }

            if (input.DashPressed)
            {
                stateMachine.ChangeState(stateMachine.DashingState);
                return;
            }

            if (player.IsTouchingWall && input.MoveInput.y > 0.1f)
            {
                stateMachine.ChangeState(stateMachine.WallRunState);
                return;
            }

            if (player.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.LocomotionState);
            }
        }

        public override void FixedUpdate()
        {
            // Air control: reduced but present
            Vector3 moveDir = GetMovementDirection();
            if (moveDir.sqrMagnitude > 0.01f)
            {
                player.RotateTowards(moveDir, 360f); // Faster rotation in air
                // Reduced air control - add force rather than set velocity
                Vector3 airForce = moveDir * player.CharacterStats.moveSpeed * 2f;
                player.Rigidbody.AddForce(airForce, ForceMode.Acceleration);
            }
        }
    }
}
