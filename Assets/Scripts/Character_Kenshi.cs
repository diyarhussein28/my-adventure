using UnityEngine;

/// <summary>
/// Kenshi: A fast, agile blade master who excels at rapid combos and precision strikes.
/// Combat Flow: High speed, low weight, combo-dependent damage scaling.
/// Unique Mechanic: "Flash Step" dodge that grants temporary invincibility and allows counterattacks.
/// </summary>
public class Character_Kenshi : BaseCharacterController
{
    [Header("Kenshi-Specific Settings")]
    public float flashStepCooldown = 3f;
    public float flashStepDuration = 0.2f;
    public float flashStepInvincibilityTime = 0.15f;
    public float comboDamageMultiplierPerStep = 0.2f; // Each hit in combo increases damage by 20%

    private float lastFlashStepTime;
    private bool isFlashStepping;
    private int currentComboStep; // Tracks current step in combo for damage scaling

    protected override void Awake()
    {
        base.Awake();
        // Kenshi starts with high speed, low weight
        if (data != null)
        {
            data.movementSpeed = 7.5f; // Faster than base
            data.weight = 0.8f; // Lighter
            data.jumpHeight = 2.5f; // Higher jump
        }
    }

    protected override void UpdateStateMachine()
    {
        base.UpdateStateMachine();
        // Override or add Kenshi-specific state logic here
        HandleFlashStepInput();
    }

    private void HandleFlashStepInput()
    {
        // Example: Flash Step on double-tap direction (simplified: Left Shift + direction)
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastFlashStepTime + flashStepCooldown)
        {
            // Get movement input
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                InitiateFlashStep(new Vector3(h, 0, v).normalized);
            }
        }
    }

    private void InitiateFlashStep(Vector3 direction)
    {
        lastFlashStepTime = Time.time;
        isFlashStepping = true;
        // Temporarily disable collisions or make invincible
        // We'll use a simple approach: set state and modify physics
        currentState = State.Dodging; // Reuse Dodging state for flash step
        // Apply instant movement
        rb.velocity = direction * data.movementSpeed * 10f; // Quick burst
        // Invincibility handled via a timer or layer change
        Invoke(nameof(EndFlashStep), flashStepDuration);
    }

    private void EndFlashStep()
    {
        isFlashStepping = false;
        // Return to idle or moving based on input
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f)
        {
            currentState = State.Moving;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    // Override combo execution to increase damage per step
    protected override void ExecuteComboStep(int step)
    {
        base.ExecuteComboStep(step);
        currentComboStep = step;
        // Optionally, modify damage multiplier based on combo step
        // This would be used in the OnHit method
    }

    // Override OnHit to apply Kenshi's combo damage scaling
    public override void OnHit(int hitIndex)
    {
        base.OnHit(hitIndex);
        // Example: Increase damage based on combo step
        float damageMultiplier = 1f + (currentComboStep * comboDamageMultiplierPerStep);
        // Apply the multiplier (this would need to be integrated into the damage system)
        Debug.Log($"Kenshi hit {hitIndex} with damage multiplier {damageMultiplier}");
    }

    // Kenshi's unique ability: Quick Draw (iaijutsu strike)
    protected override void AttemptAbility(int abilityIndex)
    {
        // Override to add Kenshi's ability logic if needed
        base.AttemptAbility(abilityIndex);
    }
}