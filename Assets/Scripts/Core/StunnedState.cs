using UnityEngine;

/// <summary>
/// State when player is stunned (e.g., by enemy attack).
/// Disables all input and movement for a set duration.
/// </summary>
public class StunnedState : PlayerState
{
    private float stunTimer;

    public StunnedState(PlayerController controller, float stunDuration) : base(controller)
    {
        stunTimer = stunDuration;
    }

    public override void Enter()
    {
        animator.SetTrigger("Stunned");
        // Zero out velocity
        controller.rb.velocity = Vector3.zero;
        controller.rb.angularVelocity = Vector3.zero;
        // Optional: disable collider or adjust for ragdoll?
    }

    public override void HandleInput()
    {
        // No input allowed during stun
    }

    public override void PhysicsUpdate()
    {
        // Optionally apply a small stun shake or keep rigidbody kinematic
        stunTimer -= Time.fixedDeltaTime;
        if (stunTimer <= 0f)
        {
            // Recover to Locomotion or Airborne
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
        animator.ResetTrigger("Stunned");
        // Optional: play recovery VFX
    }
}