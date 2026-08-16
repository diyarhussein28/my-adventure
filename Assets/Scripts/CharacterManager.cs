using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the roster of characters, loading their data and handling selection.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [Header("Character Data")]
    // We'll load all CharacterData assets from the Resources folder
    // Alternatively, we could use a list assigned in the inspector for more control
    private List<CharacterData> allCharacters = new List<CharacterData>();

    [Header("Selection")]
    public CharacterData selectedCharacter; // Currently selected character for player
    public int selectedCharacterIndex = 0;

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

        LoadAllCharacters();
    }

    private void LoadAllCharacters()
    {
        // Load all CharacterData assets from Resources/Characters
        // Make sure to place your CharacterData assets in a folder named "Characters" under Resources
        Object[] dataObjects = Resources.LoadAll("Characters", typeof(CharacterData));
        allCharacters.Clear();
        foreach (Object obj in dataObjects)
        {
            if (obj is CharacterData data)
            {
                allCharacters.Add(data);
            }
        }

        // Sort by name for consistent ordering
        allCharacters.Sort((a, b) => a.characterName.CompareTo(b.characterName));

        if (allCharacters.Count == 0)
        {
            Debug.LogWarning("No CharacterData found in Resources/Characters. Make sure to create and place them there.");
        }
        else
        {
            Debug.Log($"Loaded {allCharacters.Count} characters.");
            // Select first character by default
            if (selectedCharacter == null && allCharacters.Count > 0)
            {
                selectedCharacter = allCharacters[0];
                selectedCharacterIndex = 0;
            }
        }
    }

    /// <summary>
    /// Returns the list of all available characters.
    /// </summary>
    public List<CharacterData> GetAllCharacters() => new List<CharacterData>(allCharacters);

    /// <summary>
    /// Returns the character at the given index.
    /// </summary>
    public CharacterData GetCharacter(int index)
    {
        if (index < 0 || index >= allCharacters.Count)
        {
            Debug.LogError($"Character index {index} out of range. Max index: {allCharacters.Count - 1}");
            return null;
        }
        return allCharacters[index];
    }

    /// <summary>
    /// Returns the character with the given name (case-insensitive).
    /// </summary>
    public CharacterData GetCharacterByName(string name)
    {
        return allCharacters.Find(c => c.characterName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Selects a character by index.
    /// </summary>
    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= allCharacters.Count)
        {
            Debug.LogError($"Cannot select character index {index}. Out of range.");
            return;
        }
        selectedCharacter = allCharacters[index];
        selectedCharacterIndex = index;
        Debug.Log($"Selected character: {selectedCharacter.characterName}");
    }

    /// <summary>
    /// Selects a character by name.
    /// </summary>
    public void SelectCharacterByName(string name)
    {
        CharacterData data = GetCharacterByName(name);
        if (data != null)
        {
            SelectCharacter(GetCharacterIndex(data));
        }
        else
        {
            Debug.LogError($"Character with name '{name}' not found.");
        }
    }

    /// <summary>
    /// Returns the index of the given character data.
    /// </summary>
    public int GetCharacterIndex(CharacterData data)
    {
        return allCharacters.IndexOf(data);
    }

    /// <summary>
    /// Returns the currently selected character.
    /// </summary>
    public CharacterData GetSelectedCharacter() => selectedCharacter;

    /// <summary>
    /// Instantiates the selected character's prefab at the given position and rotation.
    /// Returns the instantiated GameObject or null if prefab is missing.
    /// </summary>
    public GameObject InstantiateSelectedCharacter(Vector3 position, Quaternion rotation)
    {
        if (selectedCharacter == null)
        {
            Debug.LogError("No character selected.");
            return null;
        }

        if (selectedCharacter.characterPrefab == null)
        {
            Debug.LogError($"Selected character {selectedCharacter.characterName} has no prefab assigned.");
            return null;
        }

        GameObject instance = Instantiate(selectedCharacter.characterPrefab, position, rotation);
        // Optionally, assign the character data to the instantiated controller
        BaseCharacterController controller = instance.GetComponent<BaseCharacterController>();
        if (controller != null)
        {
            controller.data = selectedCharacter;
            // Note: In a full setup, you'd also assign combos and abilities here based on the character's data.
            // For simplicity, we assume the prefab already has the correct references set up.
        }
        return instance;
    }

    /// <summary>
    /// Reloads all characters from resources (useful in editor).
    /// </summary>
    [ContextMenu("Reload Characters")]
    public void ReloadCharacters()
    {
        LoadAllCharacters();
    }
}