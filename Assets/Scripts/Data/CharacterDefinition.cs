using UnityEngine;

namespace SeasOfLegends.Data
{
    [CreateAssetMenu(fileName = "CharacterDefinition", menuName = "Seas of Legends/Characters/Character Definition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string characterId = "hero";
        [SerializeField] private float maxHealth = 100f;
        [SerializeField, Range(0f, 0.9f)] private float damageReduction;

        [Header("Ground Movement")]
        [SerializeField] private float moveSpeed = 6.5f;
        [SerializeField] private float acceleration = 42f;
        [SerializeField] private float rotationDegreesPerSecond = 900f;
        [SerializeField] private float jumpSpeed = 8.5f;
        [SerializeField] private float gravityMultiplier = 2.2f;
        [SerializeField] private float maxFallSpeed = 28f;

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 22f;
        [SerializeField] private float dashDuration = 0.16f;
        [SerializeField] private float dashCooldown = 0.28f;
        [SerializeField] private int maxAirDashes = 1;

        [Header("Wall Run")]
        [SerializeField] private float wallRunSpeed = 8.5f;
        [SerializeField] private float wallRunDuration = 0.75f;

        public string CharacterId => characterId;
        public float MaxHealth => maxHealth;
        public float DamageReduction => damageReduction;
        public float MoveSpeed => moveSpeed;
        public float Acceleration => acceleration;
        public float RotationDegreesPerSecond => rotationDegreesPerSecond;
        public float JumpSpeed => jumpSpeed;
        public float GravityMultiplier => gravityMultiplier;
        public float MaxFallSpeed => maxFallSpeed;
        public float DashSpeed => dashSpeed;
        public float DashDuration => dashDuration;
        public float DashCooldown => dashCooldown;
        public int MaxAirDashes => maxAirDashes;
        public float WallRunSpeed => wallRunSpeed;
        public float WallRunDuration => wallRunDuration;
    }
}
