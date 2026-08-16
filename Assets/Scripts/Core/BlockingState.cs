using UnityEngine;

/// <summary>
/// State for blocking incoming attacks.
/// Reduces damage taken and may stagger player on strong hits.
/// </summary>
public class BlockingState : PlayerState
{
    private bool isBlocking;

    public BlockingState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        isBlocking = true;
        animator.SetBool("IsBlocking", true);
        // Notify Combat Manager that blocking started
        CombatManager.Instance.SetBlocking(true);
    }

    public override void HandleInput()
    {
        // Release block when button released
        if (!controller.blockPressed)
        {
            isBlocking = false;
        }
    }

    public override void PhysicsUpdate()
    {
        // No movement while blocking (or minimal pushback)
        controller.rb.velocity = new Vector3(0, controller.rb.velocity.y, 0);

        // Exit block when button released
        if (!isBlocking)
        {
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
        animator.SetBool("IsBlocking", false);
        CombatManager.Instance.SetBlocking(false);
    }
}