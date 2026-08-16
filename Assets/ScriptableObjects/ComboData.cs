using UnityEngine;

/// <summary>
/// Defines a combo sequence: light/heavy attack strings, input buffers, and hit-stop values.
/// </summary>
[CreateAssetMenu(fileName = "NewComboData", menuName = "Game/ComboData", order = 3)]
public class ComboData : ScriptableObject
{
    [Header("Combo Name")]
    public string comboName;

    [Header("Input Sequence")]
    // Each element is a string representing the input (e.g., "Light", "Heavy", "Special")
    // We'll use an enum for simplicity, but strings allow designer flexibility.
    public string[] inputSequence; // e.g., new string[] { "Light", "Light", "Heavy" }

    [Header("Timing")]
    public float inputBufferTime = 0.3f; // How long to remember the last input for chaining
    public float[] hitStopTimes; // Hit-stop duration (in seconds) for each hit in the sequence, aligned with inputSequence
    public float[] damageMultipliers; // Damage multiplier for each hit

    [Header("Effects")]
    public float knockbackPerHit = 2f;
    public bool launchesOnLastHit = false;

    /// <summary>
    /// Number of hits in the combo.
    /// </summary>
    public int HitCount => inputSequence.Length;

    /// <summary>
    /// Validates that the arrays are aligned.
    /// </summary>
    private void OnValidate()
    {
        if (hitStopTimes == null || hitStopTimes.Length != inputSequence.Length)
        {
            hitStopTimes = new float[inputSequence.Length];
        }
        if (damageMultipliers == null || damageMultipliers.Length != inputSequence.Length)
        {
            damageMultipliers = new float[inputSequence.Length];
            for (int i = 0; i < damageMultipliers.Length; i++)
                damageMultipliers[i] = 1f;
        }
    }
}