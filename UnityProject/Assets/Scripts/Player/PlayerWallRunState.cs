using UnityEngine;

namespace SeasOfLegends.Player
{
    /// <summary>
    /// Wall-running state: player runs along vertical walls.
    /// Inspired by Demon Slayer's acrobatic movement.
    /// </summary>
    public class PlayerWallRunState : PlayerState
    {
        private float wallRunTimer = 0f;
        private Vector3 wallForward;

        public PlayerWallRunState(PlayerController player, PlayerStateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void Enter()
        {
            wallRunTimer = 0f;
            player.CanAct = false;
            player.CurrentAirDashes = 0; // Reset air dashes

            // Calculate direction along the wall
            wallForward = Vector3.Cross(player.WallNormal, Vector3.up);
            // Choose direction closest to player's input
            Vector3 inputDir = GetMovementDirection();
            if (Vector3.Dot(wallForward, inputDir) < 0)
                wallForward = -wallForward;

            player.RotateTowards(-player.WallNormal);
            player.SetVelocity(Vector3.zero);

            player.Animator?.SetBool("IsWallRunning", true);
            SeasOfLegends.Core.EventManager.Instance?.TriggerPlayerWallRunStart();
        }

        public override void Exit()
        {
            player.CanAct = true;
            player.Animator?.SetBool("IsWallRunning", false);
            SeasOfLegends.Core.EventManager.Instance?.TriggerPlayerWallRunEnd();
        }

        public override void Update()
        {
            wallRunTimer += Time.deltaTime;
            var input = SeasOfLegends.Input.InputManager.Instance;

            if (wallRunTimer >= player.CharacterStats.wallRunDuration)
            {
                stateMachine.ChangeState(stateMachine.AirborneState);
                return;
            }

            if (!player.IsTouchingWall)
            {
                stateMachine.ChangeState(stateMachine.AirborneState);
                return;
            }

            if (input.JumpPressed)
            {
                // Wall jump
                Vector3 jumpDir = (player.WallNormal + Vector3.up).normalized;
                player.SetVelocity(Vector3.zero);
                player.ApplyImpulse(jumpDir * player.CharacterStats.jumpForce * 1.2f);
                stateMachine.ChangeState(stateMachine.AirborneState);
                return;
            }

            if (input.DashPressed)
            {
                // Dash off wall
                player.Dash(-player.WallNormal);
                stateMachine.ChangeState(stateMachine.DashingState);
                return;
            }

            if (AnyAttackInput())
            {
                stateMachine.ChangeState(stateMachine.AttackingState);
                return;
            }
        }

        public override void FixedUpdate()
        {
            // Move along wall
            float speed = player.CharacterStats.wallRunSpeed;
            Vector3 wallVelocity = wallForward * speed;
            wallVelocity.y = 0.1f; // Slight upward drift
            player.SetVelocity(wallVelocity);

            // Stick to wall
            player.Rigidbody.AddForce(-player.WallNormal * 20f, ForceMode.Acceleration);
        }
    }
}
