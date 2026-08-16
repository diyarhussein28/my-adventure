using UnityEngine;

/// <summary>
/// State for executing cinematic finishing moves (e.g., fatalities).
/// Locks player control, plays special animation, and triggers camera changes.
/// </summary>
public class ExecutingState : PlayerState
{
    private float executeTimer;
    private readonly float executeDuration; // Length of the finishing move animation

    public ExecutingState(PlayerController controller, float duration) : base(controller)
    {
        executeDuration = duration;
    }

    public override void Enter()
    {
        animator.SetTrigger("Execute");
        // Lock camera to focus on enemy (handled by CombatManager or CameraController)
        CombatManager.Instance.StartExecution();
        executeTimer = executeDuration;
        // Zero velocity
        controller.rb.velocity = Vector3.zero;
    }

    public override void HandleInput()
    {
        // No input during execution
    }

    public override void PhysicsUpdate()
    {
        executeTimer -= Time.fixedDeltaTime;
        if (executeTimer <= 0f)
        {
            // Finish execution and return to Locomotion
            CombatManager.Instance.EndExecution();
            controller.ChangeState(new LocomotionState(controller));
        }
    }

    public override void Exit()
    {
        animator.ResetTrigger("Execute");
        // Optional: cleanup execution effects
    }
}