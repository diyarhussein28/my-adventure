using UnityEngine;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Grounded movement state: walking, sprinting, idle.
    /// Transitions to: Airborne (jump/fall), Attacking (attack input), 
    /// Blocking (hold block), Dashing (dash input), WallRun (touch wall + move).
    /// </summary>
    public class PlayerLocomotionState : PlayerState
    {
        public PlayerLocomotionState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            player.Animator?.SetBool("IsLocomotion", true);
            player.CanAct = true;
        }

        public override void Exit()
        {
            player.Animator?.SetBool("IsLocomotion", false);
        }

        public override void Update()
        {
            var input = SeasOfLegends.Input.InputManager.Instance;
            if (input == null) return;

            // Priority: Block > Attack > Dash > Jump > WallRun > Move

            if (input.BlockPressed)
            {
                stateMachine.ChangeState(stateMachine.BlockingState);
                return;
            }

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

            if (input.JumpPressed && player.IsGrounded)
            {
                player.Jump();
                stateMachine.ChangeState(stateMachine.AirborneState);
                return;
            }

            if (player.IsTouchingWall && input.MoveInput.y > 0.5f && !player.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.WallRunState);
                return;
            }

            // Falling off ledge
            if (!player.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.AirborneState);
            }
        }

        public override void FixedUpdate()
        {
            Vector3 moveDir = GetMovementDirection();
            player.RotateTowards(moveDir);
            player.Move(moveDir);
        }
    }
}
