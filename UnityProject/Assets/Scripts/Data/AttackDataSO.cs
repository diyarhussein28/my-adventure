using UnityEngine;

namespace SeasOfLegends.Data
{
    /// <summary>
    /// ScriptableObject defining a single attack move in a combo string.
    /// Contains frame data, hitbox parameters, and VFX references.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAttackData", menuName = "SeasOfLegends/Attack Data")]
    public class AttackDataSO : ScriptableObject
    {
        [Header("Input")]
        [Tooltip("Input button for this attack (Light/Heavy/Special)")]
        public AttackType attackType = AttackType.Light;

        [Tooltip("Input sequence string: e.g., 'L,L,H' for light-light-heavy")]
        public string comboSequence = "L";

        [Header("Frame Data (at 60fps)")]
        [Tooltip("Frames before hitbox becomes active")]
        public int startupFrames = 8;

        [Tooltip("Frames where hitbox is active and can deal damage")]
        public int activeFrames = 4;

        [Tooltip("Frames before player can act again")]
        public int recoveryFrames = 12;

        [Tooltip("Frames of advantage defender has on block (negative = unsafe)")]
        public int blockAdvantage = -6;

        [Tooltip("Frames of advantage attacker has on hit")]
        public int hitAdvantage = 4;

        [Header("Damage")]
        [Tooltip("Base damage dealt on hit")]
        public float baseDamage = 15f;

        [Tooltip("Damage multiplier when used in a combo (scales down)")]
        public float comboScaling = 0.85f;

        [Tooltip("Whether this attack launches the enemy into the air")]
        public bool isLauncher = false;

        [Tooltip("Upward force applied when launching")]
        public float launchForce = 12f;

        [Tooltip("Whether this attack is an aerial attack")]
        public bool isAerial = false;

        [Header("Hit Effects")]
        [Tooltip("Duration of time freeze on impact (seconds)")]
        public float hitStopDuration = 0.08f;

        [Tooltip("Screen shake intensity (0 = none)")]
        public float screenShakeIntensity = 0.3f;

        [Tooltip("Camera zoom amount during active frames")]
        public float cameraZoom = -2f;

        [Header("Elemental Properties")]
        public ElementType elementOverride = ElementType.None;

        [Tooltip("Chance to apply elemental status effect")]
        [Range(0f, 1f)]
        public float statusEffectChance = 0f;

        [Header("VFX References")]
        [Tooltip("Prefab spawned at hit point on impact")]
        public GameObject hitImpactVFX;

        [Tooltip("Trail VFX prefab attached to weapon during attack")]
        public GameObject weaponTrailVFX;

        [Tooltip("Sound effect played on hit")]
        public AudioClip hitSFX;

        [Tooltip("Sound effect played on whiff")]
        public AudioClip whiffSFX;

        /// <summary>
        /// Total duration of the attack in seconds (at 60fps).
        /// </summary>
        public float TotalDuration => (startupFrames + activeFrames + recoveryFrames) / 60f;

        /// <summary>
        /// Startup time in seconds.
        /// </summary>
        public float StartupTime => startupFrames / 60f;

        /// <summary>
        /// Active time in seconds.
        /// </summary>
        public float ActiveTime => activeFrames / 60f;

        /// <summary>
        /// Recovery time in seconds.
        /// </summary>
        public float RecoveryTime => recoveryFrames / 60f;
    }

    public enum AttackType
    {
        Light,      // Fast, weak, safe
        Heavy,      // Slow, strong, unsafe
        Special,    // Unique properties per character
        Grab,       // Unblockable, short range
        Ultimate    // Finisher move, cinematic
    }
}
