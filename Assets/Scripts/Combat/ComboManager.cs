using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages combo input buffering and combo string execution.
/// Works with CombatSystem to determine which attack to perform based on timed inputs.
/// </summary>
public class ComboManager
{
    // Singleton pattern
    private static ComboManager instance;
    public static ComboManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ComboManager();
            return instance;
        }
    }

    private ComboManager() { } // Prevent instantiation

    // Combo buffer settings
    [System.Serializable]
    public class ComboInput
    {
        public KeyCode key; // Or InputAction reference
        public float timeWindow; // How long after previous input this counts
    }

    // Current combo being built
    private List<KeyCode> currentBuffer = new List<KeyCode>();
    private float lastInputTime;
    private const float COMBO_RESET_TIME = 1.0f; // Time to reset combo if no input

    // Define your combos here or load from ScriptableObject
    private Dictionary<KeyCode[], string> comboLibrary = new Dictionary<KeyCode[], string>()
    {
        { new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse0, KeyCode.Mouse0 }, "BasicCombo" },
        { new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse0, KeyCode.Space }, "LauncherCombo" },
        { new KeyCode[] { KeyCode.Mouse0, KeyCode.LeftShift, KeyCode.Mouse0 }, "DashAttackCombo" }
        // Add more combos as needed
    };

    /// <summary>
    /// Call this from PlayerController's HandleInput when attack button is pressed.
    /// Returns true if the input completes a known combo.
    /// </summary>
    public bool ProcessAttackInput(KeyCode attackKey)
    {
        float timeSinceLast = Time.time - lastInputTime;

        // Reset buffer if too much time passed
        if (timeSinceLast > COMBO_RESET_TIME)
        {
            currentBuffer.Clear();
        }

        // Add current input
        currentBuffer.Add(attackKey);
        lastInputTime = Time.time;

        // Check if current buffer matches any combo
        foreach (var combo in comboLibrary)
        {
            if (currentBuffer.Count >= combo.Key.Length)
            {
                bool matches = true;
                for (int i = 0; i < combo.Key.Length; i++)
                {
                    // Check the last n inputs match the combo
                    if (currentBuffer[currentBuffer.Count - combo.Key.Length + i] != combo.Key[i])
                    {
                        matches = false;
                        break;
                    }
                }
                if (matches)
                {
                    // Found a combo! Return the combo name or ID
                    Debug.Log($"Combo executed: {combo.Value}");
                    currentBuffer.Clear(); // Reset buffer after successful combo
                    return true;
                }
            }
        }

        // Not a full combo yet, but could be part of one
        return false;
    }

    /// <summary>
    /// Called by CombatSystem to get the next attack in a sequence (for auto-combos).
    /// </summary>
    public string GetNextAttackInCombo(string currentAttack)
    {
        // Implement logic to return next attack based on current state
        // For simplicity, we'll just return a hardcoded next step
        switch (currentAttack)
        {
            case "BasicAttack1":
                return "BasicAttack2";
            case "BasicAttack2":
                return "BasicAttack3";
            default:
                return null;
        }
    }

    /// <summary>
    /// Clear the combo buffer (e.g., on hit or block).
    /// </summary>
    public void ClearBuffer()
    {
        currentBuffer.Clear();
        lastInputTime = 0f;
    }
}