using UnityEngine;

/// <summary>
/// Defines an active skill or ability for a character.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterAbility", menuName = "Game/CharacterAbility", order = 2)]
public class CharacterAbility : ScriptableObject
{
    [Header("Ability Info")]
    public string abilityName;
    public string description;
    public Sprite abilityIcon;

    [Header("Input")]
    public string inputCommand; // e.g., "Q", "E", "Mouse0", or combo like "Q,Q,E"

    [Header("Timing (frames)")]
    public int startupFrames = 5;
    public int activeFrames = 3;
    public int recoveryFrames = 10;
    // Total cooldown in seconds (can be separate from recovery)
    public float cooldown = 5f;

    [Header("Costs")]
    public float staminaCost = 20f;
    public float manaCost = 0f; // If using mana

    [Header("Effects")]
    public float damageMultiplier = 1f;
    public float knockbackForce = 5f;
    public float stunDuration = 0f;
    public bool isUltimate = false; // Ultimate moves have special handling

    [Header("VFX & SFX References")]
    public GameObject vfxPrefab;
    public AudioClip sfxClip;

    /// <summary>
    /// Returns the total animation length in frames (startup + active + recovery).
    /// </summary>
    public int TotalFrames => startupFrames + activeFrames + recoveryFrames;

    /// <summary>
    /// Returns the total animation length in seconds assuming 60 FPS.
    /// </summary>
    public float TotalSeconds => TotalFrames / 60f;
}