using UnityEngine;

/// <summary>
/// Base data for a character, stored as a ScriptableObject for easy creation and editing.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    public string title;
    public string archetype;

    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float movementSpeed = 5f;
    public float jumpHeight = 2f;
    public float weight = 1f; // Affects physics interactions
    public float attackPower = 10f;
    public float staggerResistance = 0.5f; // 0 to 1, higher means harder to stagger

    [Header("Combat")]
    public float lightAttackDamage = 10f;
    public float heavyAttackDamage = 20f;
    public float comboWindow = 0.5f; // Time window to input next combo hit

    [Header("Abilities")]
    public CharacterAbility[] abilities; // Regular abilities (slot 1-4)
    public CharacterAbility ultimateAbility; // Ultimate move (slot 5)

    [Header("Combos")]
    public ComboData[] combos; // Different combo strings (light/heavy sequences)

    [Header("References (set in inspector)")]
    public RuntimeAnimatorController animatorController;
    public GameObject characterPrefab; // Prefab for instantiating the character in the world
    public Sprite portrait; // For UI selection
}