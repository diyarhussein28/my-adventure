using System;
using UnityEngine;
using SeasOfLegends.Data;

namespace SeasOfLegends.Combat
{
    /// <summary>
    /// Per-combatant combo buffer. It records one future attack during active/recovery frames
    /// and validates that the requested input matches the next authored attack in the sequence.
    /// </summary>
    public sealed class ComboManager : MonoBehaviour
    {
        [SerializeField] private ComboDefinition[] combos;
        private ComboDefinition activeCombo;
        private int nextIndex;
        private AttackInput? bufferedInput;
        private float bufferExpiry;

        public int CurrentHitCount { get; private set; }

        /// <summary>Runtime bootstrap helper used by the starter vertical slice.</summary>
        public void ConfigureForPrototype(ComboDefinition[] prototypeCombos)
        {
            combos = prototypeCombos;
        }

        public bool TryBegin(AttackInput input, out AttackDefinition attack)
        {
            ResetCombo();
            if (combos == null)
            {
                attack = null;
                return false;
            }
            for (int i = 0; i < combos.Length; i++)
            {
                AttackDefinition[] sequence = combos[i].Sequence;
                if (sequence != null && sequence.Length > 0 && sequence[0] != null && sequence[0].Input == input)
                {
                    activeCombo = combos[i];
                    nextIndex = 1;
                    attack = sequence[0];
                    return true;
                }
            }
            attack = null;
            return false;
        }

        public void Buffer(AttackInput input)
        {
            if (activeCombo == null) return;
            bufferedInput = input;
            bufferExpiry = Time.time + activeCombo.InputBufferSeconds;
        }

        public bool TryContinue(out AttackDefinition attack)
        {
            attack = null;
            if (activeCombo == null || bufferedInput == null || Time.time > bufferExpiry) return false;
            AttackDefinition[] sequence = activeCombo.Sequence;
            if (nextIndex >= sequence.Length || sequence[nextIndex] == null || sequence[nextIndex].Input != bufferedInput.Value) return false;
            attack = sequence[nextIndex++];
            bufferedInput = null;
            return true;
        }

        public void RegisterSuccessfulHit() => CurrentHitCount++;

        public void ResetCombo()
        {
            activeCombo = null;
            nextIndex = 0;
            bufferedInput = null;
            bufferExpiry = 0f;
            CurrentHitCount = 0;
        }
    }
}
