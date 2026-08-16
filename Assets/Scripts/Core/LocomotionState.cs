using UnityEngine;

/// <summary>
/// State for ground movement: walking, running, jumping, dashing.
/// Handles acceleration, friction, and dash mechanics.
/// </summary>
public class LocomotionState : PlayerState
{
    public LocomotionState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsDashing", false);
        dashTimeRemaining = 0f;
    }

    public override void HandleInput()
    {
        // Jump
        if (controller.jumpPressed && controller.IsGrounded())
        {
            controller.ChangeState(new AirborneState(controller));
            return;
        }

        // Dash (only when moving)
        if (controller.dashPressed && controller.moveInput != Vector2.zero && dashTimeRemaining <= 0f)
        {
            controller.ChangeState(new DashingState(controller)); // We'll create DashingState as a substate or separate
            return;
        }

        // Attack
        if (controller.attackPressed)
        {
            controller.ChangeState(new AttackingState(controller));
            return;
        }

        // Block
        if (controller.blockPressed)
        {
            controller.ChangeState(new BlockingState(controller));
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        Vector3 moveDir = controller.GetMoveDirection();
        bool isMoving = moveDir.sqrMagnitude > 0.01f;

        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // Apply movement force
            Vector3 targetVelocity = moveDir * controller.moveSpeed;
            // Smoothly change velocity (avoid instant changes)
            Vector3 velocityChange = (targetVelocity - controller.rb.velocity);
            velocityChange.y = 0; // Only change horizontal velocity
            controller.rb.AddForce(velocityChange * controller.rb.mass, ForceMode.Acceleration);
        }
        else
        {
            // Apply friction when not moving
            Vector3 horizontalVel = controller.rb.velocity;
            horizontalVel.y = 0;
            if (horizontalVel.sqrMagnitude > 0.001f)
            {
                float drag = 10f; // High drag to stop quickly
                Vector3 frictionForce = -horizontalVel.normalized * drag * controller.rb.mass;
                controller.rb.AddForce(frictionForce, ForceMode.Acceleration);
            }
        }

        // Dash timer
        if (dashTimeRemaining > 0f)
        {
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining <= 0f)
            {
                animator.SetBool("IsDashing", false);
            }
        }
    }

    public override void Exit()
    {
        // Ensure dash is turned off
        animator.SetBool("IsDashing", false);
    }
}