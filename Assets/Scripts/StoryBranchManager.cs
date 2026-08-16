using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the branching narrative paths for each character, tracking choices and triggering events.
/// </summary>
public class StoryBranchManager : MonoBehaviour
{
    public static StoryBranchManager Instance { get; private set; }

    // Prefix for PlayerPrefs keys to avoid conflicts
    private const string PREF_PREFIX = "Game_Narrative_";

    // Selected character name (could also be an enum or index)
    private string selectedCharacter;

    // Dictionary to hold narrative flags for each character
    // In a full game, you might have a more complex structure (like a state machine per character)
    private Dictionary<string, Dictionary<string, bool>> characterFlags = new Dictionary<string, Dictionary<string, bool>>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadNarrativeState();
    }

    private void LoadNarrativeState()
    {
        // Load selected character
        selectedCharacter = PlayerPrefs.GetString(PREF_PREFIX + "SelectedCharacter", "");

        // Load flags for each character (we'll load on demand when needed)
        // For simplicity, we'll assume we know the character names from the roster
        // In practice, you'd load all known characters from a list
        LoadFlagsForCharacter("Kenshi");
        LoadFlagsForCharacter("Valeria");
        LoadFlagsForCharacter("Lyra");
        LoadFlagsForCharacter("Baron");
        LoadFlagsForCharacter("Zane");
        LoadFlagsForCharacter("Juno");
        LoadFlagsForCharacter("Astrid");
        LoadFlagsForCharacter("Dante");
        LoadFlagsForCharacter("Mira");
        LoadFlagsForCharacter("Orion");
    }

    private void LoadFlagsForCharacter(string characterName)
    {
        if (!characterFlags.ContainsKey(characterName))
        {
            characterFlags[characterName] = new Dictionary<string, bool>();
        }

        // Example flags: you would define specific flag names for each character's story
        // For brevity, we'll just note that flags can be set and retrieved
        // In a real game, you'd have a predefined list of flags per character
        // Here we'll dynamically handle any flag that is set
    }

    /// <summary>
    /// Sets the currently selected character for the player.
    /// </summary>
    public void SetSelectedCharacter(string characterName)
    {
        selectedCharacter = characterName;
        PlayerPrefs.SetString(PREF_PREFIX + "SelectedCharacter", characterName);
        PlayerPrefs.Save();
        Debug.Log($"Narrative: Selected character set to {characterName}");
    }

    /// <summary>
    /// Gets the currently selected character.
    /// </summary>
    public string GetSelectedCharacter() => selectedCharacter;

    /// <summary>
    /// Sets a narrative flag for the currently selected character.
    /// </summary>
    public void SetFlag(string flagName, bool value)
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.LogWarning("No character selected. Cannot set narrative flag.");
            return;
        }

        if (!characterFlags.ContainsKey(selectedCharacter))
        {
            characterFlags[selectedCharacter] = new Dictionary<string, bool>();
        }

        characterFlags[selectedCharacter][flagName] = value;
        // Save to PlayerPrefs
        string key = PREF_PREFIX + selectedCharacter + "_" + flagName;
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Gets a narrative flag for the currently selected character.
    /// Returns false if the flag doesn't exist (default).
    /// </summary>
    public bool GetFlag(string flagName)
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.LogWarning("No character selected. Cannot get narrative flag.");
            return false;
        }

        if (!characterFlags.ContainsKey(selectedCharacter) ||
            !characterFlags[selectedCharacter].ContainsKey(flagName))
        {
            // Try to load from PlayerPrefs
            string key = PREF_PREFIX + selectedCharacter + "_" + flagName;
            int value = PlayerPrefs.GetInt(key, -1);
            if (value != -1)
            {
                bool boolValue = value == 1;
                // Cache it
                if (!characterFlags.ContainsKey(selectedCharacter))
                    characterFlags[selectedCharacter] = new Dictionary<string, bool>();
                characterFlags[selectedCharacter][flagName] = boolValue;
                return boolValue;
            }
            return false; // Default
        }

        return characterFlags[selectedCharacter][flagName];
    }

    /// <summary>
    /// Resets all narrative data (for debugging or new game).
    /// </summary>
    [ContextMenu("Reset Narrative Data")]
    public void ResetNarrativeData()
    {
        selectedCharacter = "";
        characterFlags.Clear();
        PlayerPrefs.DeleteKey(PREF_PREFIX + "SelectedCharacter");
        // Note: In a real game, you'd delete all keys with the prefix
        // For simplicity, we'll just clear the known ones (not implemented here)
        PlayerPrefs.Save();
        Debug.Log("Narrative data reset.");
    }

    // -------------------------------------------------
    // Event Triggering Methods
    // -------------------------------------------------

    /// <summary>
    /// Checks if a specific island event should trigger based on the selected character and their flags.
    /// Returns true if the event was triggered.
    /// </summary>
    public bool TriggerIslandEvent(string islandEventName)
    {
        if (string.IsNullOrEmpty(selectedCharacter))
            return false;

        switch (islandEventName)
        {
            case "Whispering Coves":
                // Example: Trigger for Kenshi if they have chosen the Path of the Whispering Wind
                return GetFlag("Kenshi_Path_WhisperingWind") && TryTriggerEvent(islandEventName);
            case "Volcanic Forge":
                // Example: Trigger for Valeria if they have chosen the Path of the Molten Core
                return GetFlag("Valeria_Path_MoltenCore") && TryTriggerEvent(islandEventName);
            case "Starlight Observatory":
                // Example: Trigger for Lyra if they have chosen the Path of the Shooting Star
                return GetFlag("Lyra_Path_ShootingStar") && TryTriggerEvent(islandEventName);
            case "Abyssal Trench":
                // Example: Trigger for Baron if they have chosen the Path of the Leviathan's Embrace
                return GetFlag("Baron_Path_LeviathanEmbrace") && TryTriggerEvent(islandEventName);
            case "Void Rift":
                // Example: Trigger for Zane if they have chosen the Path of the Void Walker
                return GetFlag("Zane_Path_VoidWalker") && TryTriggerEvent(islandEventName);
            case "Storm Spire":
                // Example: Trigger for Juno if they have chosen the Path of the Wind Dancer
                return GetFlag("Juno_Path_WindDancer") && TryTriggerEvent(islandEventName);
            case "Ember Sanctum":
                // Example: Trigger for Astrid if they have chosen the Path of the Berserker's Rage
                return GetFlag("Astrid_Path_BerserkerRage") && TryTriggerEvent(islandEventName);
            case "Gravity Observatory":
                // Example: Trigger for Dante if they have chosen the Path of the Attractor
                return GetFlag("Dante_Path_Attractor") && TryTriggerEvent(islandEventName);
            case "Chronos Vault":
                // Example: Trigger for Mira if they have chosen the Path of the Slow Warden
                return GetFlag("Mira_Path_SlowWarden") && TryTriggerEvent(islandEventName);
            case "Starfall Temple":
                // Example: Trigger for Orion if they have chosen the Path of the Radiant Guardian
                return GetFlag("Orion_Path_RadiantGuardian") && TryTriggerEvent(islandEventName);
            default:
                Debug.LogWarning($"Unknown island event: {islandEventName}");
                return false;
        }
    }

    private bool TryTriggerEvent(string eventName)
    {
        // In a full implementation, this would activate the actual event in the game world
        Debug.Log($"Narrative Event Triggered: {eventName} for character {selectedCharacter}");
        // You could activate a GameObject, start a cutscene, etc.
        return true;
    }

    // Example methods to set character-specific path flags (would be called by choice systems)
    public void SetKenshiPath(string pathName)
    {
        // Reset all Kenshi path flags
        SetFlag("Kenshi_Path_WhisperingWind", false);
        SetFlag("Kenshi_Path_StormFury", false);
        SetFlag("Kenshi_Path_LoneBlade", false);
        // Set the chosen path
        switch (pathName)
        {
            case "WhisperingWind": SetFlag("Kenshi_Path_WhisperingWind", true); break;
            case "StormFury": SetFlag("Kenshi_Path_StormFury", true); break;
            case "LoneBlade": SetFlag("Kenshi_Path_LoneBlade", true); break;
        }
    }

    // Similar methods for other characters would be implemented here...
    // For brevity, we'll just show one more example
    public void SetValeriaPath(string pathName)
    {
        SetFlag("Valeria_Path_MoltenCore", false);
        SetFlag("Valeria_Path_CrystalSentinel", false);
        SetFlag("Valeria_Path_EchoingTremor", false);
        switch (pathName)
        {
            case "MoltenCore": SetFlag("Valeria_Path_MoltenCore", true); break;
            case "CrystalSentinel": SetFlag("Valeria_Path_CrystalSentinel", true); break;
            case "EchoingTremor": SetFlag("Valeria_Path_EchoingTremor", true); break;
        }
    }
}