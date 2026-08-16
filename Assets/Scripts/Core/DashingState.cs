using UnityEngine;

/// <summary>
/// Short-lived state for high-speed dashing.
/// Disables player control for a fixed duration, applies dash velocity.
/// </summary>
public class DashingState : PlayerState
{
    private float dashTimeRemaining;

    public DashingState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        animator.SetBool("IsDashing", true);
        dashTimeRemaining = controller.dashDuration;

        // Store dash direction based on current move input or forward
        Vector3 dashDir = controller.GetMoveDirection();
        if (dashDir.sqrMagnitude < 0.01f)
        {
            dashDir = controller.transform.forward; // Default forward if no input
            dashDir.y = 0;
        }
        controller.dashDirection = dashDir.normalized;
    }

    public override void PhysicsUpdate()
    {
        // Apply dash velocity (no player control during dash)
        Vector3 dashVelocity = controller.dashDirection * controller.dashSpeed;
        // Preserve vertical velocity (for jumps/dashes off ledges)
        dashVelocity.y = controller.rb.velocity.y;
        controller.rb.velocity = dashVelocity;

        dashTimeRemaining -= Time.fixedDeltaTime;
        if (dashTimeRemaining <= 0f)
        {
            // Transition back to Locomotion (or Airborne if not grounded)
            if (controller.IsGrounded())
            {
                controller.ChangeState(new LocomotionState(controller));
            }
            else
            {
                controller.ChangeState(new AirborneState(controller));
            }
        }
    }

    public override void Exit()
    {
        animator.SetBool("IsDashing", false);
        // Optional: add dash cooldown or particle effect
    }
}