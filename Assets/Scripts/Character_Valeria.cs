using UnityEngine;

/// <summary>
/// Valeria: A heavy-hitting elemental brawler who channels primal forces into devastating blows.
/// Combat Flow: Slow movement, high weight, high damage, area-of-effect attacks.
/// Unique Mechanic: "Overcharge" system - holding attack buttons charges up attacks for greater damage and area.
/// </summary>
public class Character_Valeria : BaseCharacterController
{
    [Header("Valeria-Specific Settings")]
    public float overchargeTime = 1.5f; // Time to fully charge an attack
    public float overchargeDamageMultiplier = 2f; // Max damage multiplier when fully charged
    public float overchargeSizeMultiplier = 1.5f; // Increases area of effect
    public float stunResistanceBoost = 0.3f; // Increases stagger resistance when overcharging

    private bool isOvercharging;
    private float overchargeStartTime;
    private int overchargeAbilityIndex = -1; // Which ability is being overcharged

    protected override void Awake()
    {
        base.Awake();
        // Valeria is heavy and strong
        if (data != null)
        {
            data.movementSpeed = 3.5f; // Slower
            data.weight = 2.0f; // Heavy
            data.jumpHeight = 1.5f; // Lower jump
            data.attackPower = 15f; // Higher base attack
            data.staggerResistance = 0.7f; // Harder to stagger
        }
    }

    protected override void UpdateStateMachine()
    {
        base.UpdateStateMachine();
        HandleOverchargeInput();
    }

    private void HandleOverchargeInput()
    {
        // Example: Overcharge light attack (Fire1) or heavy attack (Fire2)
        if (Input.GetKeyDown(KeyCode.Mouse0)) // Light attack
        {
            StartOvercharge(0); // Assuming ability index 0 is a melee attack
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && isOvercharging)
        {
            ReleaseOvercharge();
        }

        // For simplicity, we'll treat heavy attack as another ability
        if (Input.GetKeyDown(KeyCode.Mouse1)) // Heavy attack / Ability
        {
            StartOvercharge(1); // Ability index 1
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) && isOvercharging)
        {
            ReleaseOvercharge();
        }
    }

    private void StartOvercharge(int abilityIndex)
    {
        if (isOvercharging) return; // Already overcharging

        isOvercharging = true;
        overchargeStartTime = Time.time;
        overchargeAbilityIndex = abilityIndex;

        // Visual feedback: change color, aura, etc.
        // For now, we'll just log
        Debug.Log($"Valeria starts overcharging ability {abilityIndex}");

        // Increase stagger resistance while charging
        if (data != null)
        {
            data.staggerResistance = Mathf.Min(0.95f, data.staggerResistance + stunResistanceBoost);
        }
    }

    private void ReleaseOvercharge()
    {
        if (!isOvercharging) return;

        float chargeDuration = Time.time - overchargeStartTime;
        float chargePercentage = Mathf.Clamp01(chargeDuration / overchargeTime);

        // Apply overcharge effects
        float damageMultiplier = 1f + (chargePercentage * (overchargeDamageMultiplier - 1f));
        float sizeMultiplier = 1f + (chargePercentage * (overchargeSizeMultiplier - 1f));

        Debug.Log($"Valeria releases overcharge! Charge: {chargePercentage*100}% -> Damage x{damageMultiplier}, Size x{sizeMultiplier}");

        // Here we would trigger the ability with the overcharge modifiers
        // For simplicity, we'll just call the base ability with a note
        // In a full implementation, we'd pass the modifiers to the ability execution
        if (overchargeAbilityIndex >= 0 && overchargeAbilityIndex < abilities.Length)
        {
            // We'll trigger the ability but with overcharge stats
            // This requires modifying the ability system to accept overrides
            // For now, we'll just call the base method and log
            Debug.Log($"Would trigger ability {overchargeAbilityIndex} with overcharge mods");
            base.AttemptAbility(overchargeAbilityIndex);
        }

        // Reset overcharge state
        isOvercharging = false;
        overchargeAbilityIndex = -1;

        // Reset stagger resistance
        if (data != null)
        {
            data.staggerResistance = Mathf.Max(0.5f, data.staggerResistance - stunResistanceBoost);
        }
    }

    // Valeria's abilities often have area of effect; we can modify the OnHit to apply splash damage
    public override void OnHit(int hitIndex)
    {
        base.OnHit(hitIndex);
        // If overcharged, apply area damage
        if (isOvercharging)
        {
            // In reality, we'd calculate splash damage here
            Debug.Log($"Valeria's overcharged hit {hitIndex} applies splash damage");
        }
    }

    // Valeria's ultimate: Elemental Cataclysm
    // Could override ultimate ability triggering
}