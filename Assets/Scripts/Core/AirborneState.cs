using UnityEngine;

/// <summary>
/// State for when player is in the air (jumping, falling).
/// Allows air control and handling of jump/dash/attack inputs.
/// </summary>
public class AirborneState : PlayerState
{
    public AirborneState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        animator.SetBool("IsGrounded", false);
        // Reset jump flag so we don't double jump on land
        controller.jumpPressed = false;
    }

    public override void HandleInput()
    {
        // Allow another jump if we have extra jumps (e.g., double jump)
        // For simplicity, we'll just allow one extra jump (could be extended)
        if (controller.jumpPressed && !controller.IsGrounded())
        {
            // Example: double jump
            controller.rb.velocity = new Vector3(controller.rb.velocity.x, controller.jumpForce, controller.rb.velocity.z);
            controller.jumpPressed = false; // Consume input
            // Could trigger second jump VFX here
        }

        // Dash in air (if allowed)
        if (controller.dashPressed && controller.moveInput != Vector2.zero && controller.dashTimeRemaining <= 0f)
        {
            controller.ChangeState(new DashingState(controller));
            return;
        }

        // Attack in air (for aerial combos)
        if (controller.attackPressed)
        {
            controller.ChangeState(new AttackingState(controller));
            return;
        }

        // Block in air (could deflect projectiles)
        if (controller.blockPressed)
        {
            controller.ChangeState(new BlockingState(controller));
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        // Air control: reduced acceleration compared to ground
        Vector3 moveDir = controller.GetMoveDirection();
        if (moveDir.sqrMagnitude > 0.01f)
        {
            float airAcceleration = 20f; // Lower than ground for floaty feel
            Vector3 targetVelocity = moveDir * controller.moveSpeed * 0.5f; // Reduced air speed
            Vector3 velocityChange = (targetVelocity - controller.rb.velocity);
            velocityChange.y = 0; // Only horizontal
            controller.rb.AddForce(velocityChange * controller.rb.mass * airAcceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        // Check for landing
        if (controller.IsGrounded())
        {
            controller.ChangeState(new LocomotionState(controller));
            controller.OnLand(); // Trigger land effects
        }
    }

    public override void Exit()
    {
        animator.SetBool("IsGrounded", true);
    }
}