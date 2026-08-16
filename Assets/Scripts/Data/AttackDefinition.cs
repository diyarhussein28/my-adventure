using UnityEngine;

namespace SeasOfLegends.Data
{
    public enum AttackInput { Light, Heavy, Special, Grab, Ultimate }

    [CreateAssetMenu(fileName = "AttackDefinition", menuName = "Seas of Legends/Combat/Attack Definition")]
    public sealed class AttackDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string attackId = "light_1";
        [SerializeField] private AttackInput input;
        [SerializeField] private string animatorTrigger = "Attack_Light_1";

        [Header("Frame Data at 60 FPS")]
        [Min(0), SerializeField] private int startupFrames = 8;
        [Min(1), SerializeField] private int activeFrames = 3;
        [Min(0), SerializeField] private int recoveryFrames = 15;
        [SerializeField] private int hitAdvantageFrames = 4;
        [SerializeField] private int blockAdvantageFrames = -3;

        [Header("Impact")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float comboScale = 0.9f;
        [SerializeField] private float hitStopSeconds = 0.06f;
        [SerializeField] private float knockbackForce = 6f;
        [SerializeField] private float launchForce;
        [SerializeField] private GameObject impactVfx;

        public string AttackId => attackId;
        public AttackInput Input => input;
        public string AnimatorTrigger => animatorTrigger;
        public int StartupFrames => startupFrames;
        public int ActiveFrames => activeFrames;
        public int RecoveryFrames => recoveryFrames;
        public int HitAdvantageFrames => hitAdvantageFrames;
        public int BlockAdvantageFrames => blockAdvantageFrames;
        public float BaseDamage => baseDamage;
        public float ComboScale => comboScale;
        public float HitStopSeconds => hitStopSeconds;
        public float KnockbackForce => knockbackForce;
        public float LaunchForce => launchForce;
        public GameObject ImpactVfx => impactVfx;

        // Frame count / simulation rate converts authored fighting-game data to seconds.
        public float StartupSeconds => startupFrames / 60f;
        public float ActiveSeconds => activeFrames / 60f;
        public float RecoverySeconds => recoveryFrames / 60f;
        public float TotalSeconds => (startupFrames + activeFrames + recoveryFrames) / 60f;

        /// <summary>Runtime bootstrap helper for sample attacks without serialized assets.</summary>
        public void ConfigureForPrototype(string id, AttackInput attackInput, float damage, int startup, int active, int recovery)
        {
            attackId = id;
            input = attackInput;
            baseDamage = damage;
            startupFrames = Mathf.Max(0, startup);
            activeFrames = Mathf.Max(1, active);
            recoveryFrames = Mathf.Max(0, recovery);
        }
    }

    [CreateAssetMenu(fileName = "ComboDefinition", menuName = "Seas of Legends/Combat/Combo Definition")]
    public sealed class ComboDefinition : ScriptableObject
    {
        [SerializeField] private string comboId = "light_chain";
        [SerializeField] private AttackDefinition[] sequence;
        [Min(0.05f), SerializeField] private float inputBufferSeconds = 0.3f;

        public string ComboId => comboId;
        public AttackDefinition[] Sequence => sequence;
        public float InputBufferSeconds => inputBufferSeconds;

        /// <summary>Runtime bootstrap helper for a self-contained sample combo.</summary>
        public void ConfigureForPrototype(string id, AttackDefinition[] attacks, float bufferSeconds = 0.3f)
        {
            comboId = id;
            sequence = attacks;
            inputBufferSeconds = Mathf.Max(0.05f, bufferSeconds);
        }
    }
}
