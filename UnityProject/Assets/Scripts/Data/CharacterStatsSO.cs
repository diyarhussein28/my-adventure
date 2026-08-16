using UnityEngine;

namespace SeasOfLegends.Data
{
    /// <summary>
    /// ScriptableObject containing all base stats for a character.
    /// Used by both player and enemies for consistent stat calculations.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacterStats", menuName = "SeasOfLegends/Character Stats")]
    public class CharacterStatsSO : ScriptableObject
    {
        [Header("Base Vitality")]
        [Tooltip("Maximum health points")]
        public float maxHealth = 100f;

        [Tooltip("Health regenerated per second out of combat")]
        public float healthRegen = 2f;

        [Tooltip("Damage reduction percentage (0-1)")]
        [Range(0f, 0.95f)]
        public float defense = 0f;

        [Header("Movement")]
        [Tooltip("Ground movement speed in units/second")]
        public float moveSpeed = 6f;

        [Tooltip("Sprint multiplier applied to moveSpeed")]
        public float sprintMultiplier = 1.6f;

        [Tooltip("Dash distance in units")]
        public float dashDistance = 8f;

        [Tooltip("Dash cooldown in seconds")]
        public float dashCooldown = 1.5f;

        [Tooltip("Dash duration in seconds (determines speed)")]
        public float dashDuration = 0.15f;

        [Tooltip("Number of consecutive air dashes allowed")]
        public int maxAirDashes = 1;

        [Tooltip("Jump force applied to Rigidbody")]
        public float jumpForce = 12f;

        [Tooltip("Gravity multiplier for snappy falls")]
        public float gravityMultiplier = 2.5f;

        [Tooltip("Terminal velocity cap")]
        public float maxFallSpeed = 50f;

        [Tooltip("Wall-run duration in seconds")]
        public float wallRunDuration = 2f;

        [Tooltip("Wall-run speed")]
        public float wallRunSpeed = 8f;

        [Header("Combat")]
        [Tooltip("Base attack damage multiplier")]
        public float attackPower = 10f;

        [Tooltip("Critical hit chance (0-1)")]
        [Range(0f, 1f)]
        public float critChance = 0.05f;

        [Tooltip("Critical hit damage multiplier")]
        public float critMultiplier = 2f;

        [Tooltip("Stun resistance (reduces stun duration, 0-1)")]
        [Range(0f, 1f)]
        public float stunResistance = 0f;

        [Header("Elemental Affinity")]
        public ElementType primaryElement = ElementType.None;
        public ElementType secondaryElement = ElementType.None;

        [Tooltip("Elemental damage bonus percentage")]
        public float elementalDamageBonus = 0f;

        [Header("Experience & Progression")]
        public int baseXP = 100;
        public int level = 1;
    }
}
