using UnityEngine;

/// <summary>
/// State for performing attacks. Interfaces with Combat System to execute combos,
/// hitbox activation, and hit-pause effects.
/// </summary>
public class AttackingState : PlayerState
{
    private bool comboInputBuffered;
    private float attackExitTime; // Time to auto-exit attack state

    public AttackingState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        // Reset combo buffer on new attack
        comboInputBuffered = false;
        // Notify Combat Manager to start attack sequence
        CombatManager.Instance.StartAttack();
        // Set attack exit time based on current attack clip length (could be from anim)
        attackExitTime = Time.time + 0.5f; // Placeholder
    }

    public override void HandleInput()
    {
        // Buffer combo input during attack window (early in animation)
        if (controller.attackPressed && Time.time < attackExitTime - 0.2f) // Buffer window
        {
            comboInputBuffered = true;
        }
    }

    public override void PhysicsUpdate()
    {
        // During attack, root motion from animation usually drives movement.
        // For simplicity, we'll zero horizontal velocity unless using root motion.
        Vector3 velocity = controller.rb.velocity;
        velocity.x = 0;
        velocity.z = 0;
        controller.rb.velocity = velocity;

        // Check if we should continue to next combo or exit
        if (Time.time >= attackExitTime)
        {
            if (comboInputBuffered)
            {
                // Stay in attack state for next combo
                CombatManager.Instance.ContinueCombo();
                attackExitTime = Time.time + 0.5f; // Reset timer
                comboInputBuffered = false;
            }
            else
            {
                // Exit to Locomotion or Airborne
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
    }

    public override void Exit()
    {
        // Notify Combat Manager that attack sequence ended
        CombatManager.Instance.EndAttack();
    }
}