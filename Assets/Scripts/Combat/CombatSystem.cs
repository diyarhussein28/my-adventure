using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasOfLegends.Core;
using SeasOfLegends.Data;
using SeasOfLegends.Player;

namespace SeasOfLegends.Combat
{
    /// <summary>
    /// Scene-level combat coordinator. Assign it to a persistent CombatSystems object. It maps
    /// startup, active, and recovery phases from AttackDefinition to a weapon Hitbox.
    /// </summary>
    public sealed class CombatSystem : MonoBehaviour
    {
        private sealed class AttackRuntime
        {
            public AttackDefinition Attack;
            public ComboManager Combo;
            public Hitbox Hitbox;
            public float StartedAt;
            public bool IsArmed;
        }

        [SerializeField, Range(0f, 0.25f)] private float blockedHitStopSeconds = 0.025f;
        [SerializeField, Range(0f, 1f)] private float hitStopTimeScale = 0.04f;
        private readonly Dictionary<PlayerController, AttackRuntime> activeAttacks = new Dictionary<PlayerController, AttackRuntime>();
        private Coroutine hitStopRoutine;

        public bool TryStartAttack(PlayerController player, AttackInput input)
        {
            ComboManager combo = player.GetComponent<ComboManager>();
            Hitbox hitbox = player.GetComponentInChildren<Hitbox>(true);
            if (combo == null || hitbox == null || !combo.TryBegin(input, out AttackDefinition attack)) return false;
            BeginRuntime(player, combo, hitbox, attack);
            return true;
        }

        public bool TickAttack(PlayerController player)
        {
            if (!activeAttacks.TryGetValue(player, out AttackRuntime runtime)) return true;
            if (player.Input.HasAttackPressed) runtime.Combo.Buffer(player.Input.BufferedAttack);

            float elapsed = Time.time - runtime.StartedAt;
            float activeStart = runtime.Attack.StartupSeconds;
            float activeEnd = activeStart + runtime.Attack.ActiveSeconds;
            if (!runtime.IsArmed && elapsed >= activeStart)
            {
                runtime.IsArmed = true;
                runtime.Hitbox.Arm(this, player.gameObject, runtime.Attack, runtime.Combo.CurrentHitCount + 1);
            }
            if (runtime.IsArmed && elapsed >= activeEnd)
            {
                runtime.IsArmed = false;
                runtime.Hitbox.Disarm();
            }
            if (elapsed < runtime.Attack.TotalSeconds) return false;

            if (runtime.Combo.TryContinue(out AttackDefinition next))
            {
                BeginRuntime(player, runtime.Combo, runtime.Hitbox, next);
                return false;
            }
            activeAttacks.Remove(player);
            runtime.Combo.ResetCombo();
            return true;
        }

        public void EndAttack(PlayerController player)
        {
            if (!activeAttacks.TryGetValue(player, out AttackRuntime runtime)) return;
            runtime.Hitbox.Disarm();
            activeAttacks.Remove(player);
        }

        public void ResolveHit(GameObject attacker, Combatant defender, AttackDefinition attack, Vector3 point, Vector3 direction, int comboCount)
        {
            defender.ApplyHit(attacker, attack, point, direction, comboCount);
            ComboManager combo = attacker.GetComponent<ComboManager>();
            combo?.RegisterSuccessfulHit();
            if (attack.ImpactVfx != null) Instantiate(attack.ImpactVfx, point, Quaternion.LookRotation(-direction));
            StartHitStop(defender.IsBlocking ? blockedHitStopSeconds : attack.HitStopSeconds);
        }

        private void BeginRuntime(PlayerController player, ComboManager combo, Hitbox hitbox, AttackDefinition attack)
        {
            hitbox.Disarm();
            activeAttacks[player] = new AttackRuntime { Attack = attack, Combo = combo, Hitbox = hitbox, StartedAt = Time.time };
            Animator animator = player.GetComponent<Animator>();
            if (!string.IsNullOrEmpty(attack.AnimatorTrigger)) animator.SetTrigger(attack.AnimatorTrigger);
        }

        private void StartHitStop(float seconds)
        {
            if (seconds <= 0f) return;
            if (hitStopRoutine != null) StopCoroutine(hitStopRoutine);
            hitStopRoutine = StartCoroutine(HitStop(seconds));
        }

        private IEnumerator HitStop(float seconds)
        {
            float originalScale = Time.timeScale;
            float originalFixedDelta = Time.fixedDeltaTime;
            Time.timeScale = hitStopTimeScale;
            Time.fixedDeltaTime = originalFixedDelta * hitStopTimeScale;
            yield return new WaitForSecondsRealtime(seconds);
            Time.timeScale = originalScale;
            Time.fixedDeltaTime = originalFixedDelta;
            hitStopRoutine = null;
        }
    }
}
