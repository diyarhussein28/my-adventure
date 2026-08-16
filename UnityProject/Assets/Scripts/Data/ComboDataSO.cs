using UnityEngine;

namespace SeasOfLegends.Data
{
    /// <summary>
    /// ScriptableObject containing a full combo string definition.
    /// Maps input sequences to AttackDataSO assets.
    /// </summary>
    [CreateAssetMenu(fileName = "NewComboData", menuName = "SeasOfLegends/Combo Data")]
    public class ComboDataSO : ScriptableObject
    {
        [Header("Combo Identity")]
        [Tooltip("Display name of the combo")]
        public string comboName = "Basic Combo";

        [Tooltip("Description shown in UI")]
        public string description = "A simple 3-hit combo";

        [Header("Input Sequence")]
        [Tooltip("Ordered list of attack inputs that define this combo")]
        public AttackDataSO[] attackSequence;

        [Header("Combo Properties")]
        [Tooltip("Maximum time between inputs before combo resets (seconds)")]
        public float inputBufferWindow = 0.35f;

        [Tooltip("Damage multiplier applied to the entire combo")]
        public float comboDamageMultiplier = 1.2f;

        [Tooltip("Whether this combo ends with a launcher")]
        public bool endsWithLauncher = false;

        [Tooltip("Whether this combo can be performed in the air")]
        public bool aerialCombo = false;

        [Header("Finisher Properties")]
        [Tooltip("If true, final hit triggers cinematic finisher on kill")]
        public bool isFinisherCombo = false;

        [Tooltip("Cinematic camera path for finisher")]
        public AnimationClip finisherCameraAnimation;

        [Tooltip("Total duration of finisher cinematic")]
        public float finisherDuration = 3f;

        /// <summary>
        /// Calculates total damage of the combo at full scaling.
        /// </summary>
        public float CalculateTotalDamage()
        {
            float total = 0f;
            float scaling = 1f;

            foreach (var attack in attackSequence)
            {
                total += attack.baseDamage * scaling;
                scaling *= attack.comboScaling;
            }

            return total * comboDamageMultiplier;
        }

        /// <summary>
        /// Total duration of the combo in seconds.
        /// </summary>
        public float TotalDuration
        {
            get
            {
                float duration = 0f;
                foreach (var attack in attackSequence)
                    duration += attack.TotalDuration;
                return duration;
            }
        }
    }
}
