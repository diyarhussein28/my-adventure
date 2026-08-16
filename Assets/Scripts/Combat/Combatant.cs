using UnityEngine;
using SeasOfLegends.Core;
using SeasOfLegends.Data;
using SeasOfLegends.Player;
using SeasOfLegends.AI;

namespace SeasOfLegends.Combat
{
    /// <summary>
    /// Required components: Collider and optionally Rigidbody. Attach to every actor that can
    /// receive a Hitbox. Blocking is supplied by PlayerController or an enemy controller.
    /// </summary>
    public sealed class Combatant : MonoBehaviour
    {
        [SerializeField] private float maximumHealth = 100f;
        [SerializeField, Range(0f, 0.9f)] private float damageReduction;
        [SerializeField] private bool canBeLaunched = true;

        public float CurrentHealth { get; private set; }
        public bool IsDefeated => CurrentHealth <= 0f;
        public bool IsBlocking
        {
            get
            {
                PlayerController player = GetComponent<PlayerController>();
                return player != null && player.IsBlocking;
            }
        }

        private void Awake() => CurrentHealth = maximumHealth;

        public void ApplyHit(GameObject attacker, AttackDefinition attack, Vector3 point, Vector3 direction, int comboCount)
        {
            if (IsDefeated) return;
            bool blocked = IsBlocking && Vector3.Dot(transform.forward, -direction.normalized) > 0.1f;
            float scaling = Mathf.Pow(Mathf.Clamp01(attack.ComboScale), Mathf.Max(0, comboCount - 1));
            float damage = attack.BaseDamage * scaling * (1f - damageReduction);
            if (blocked) damage *= 0.2f;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            bool launched = !blocked && canBeLaunched && attack.LaunchForce > 0f;
            if (rigidbody != null && !blocked)
            {
                Vector3 force = direction.normalized * attack.KnockbackForce;
                if (launched) force += Vector3.up * attack.LaunchForce;
                rigidbody.AddForce(force, ForceMode.Impulse);
            }

            PlayerController player = GetComponent<PlayerController>();
            EnemyController enemy = GetComponent<EnemyController>();
            if (player != null && !blocked) player.ReceiveHit(Mathf.Max(0f, attack.HitAdvantageFrames) / 60f);
            if (enemy != null && !blocked) enemy.ReceiveHitStun(Mathf.Max(0f, attack.HitAdvantageFrames) / 60f);
            GameEvents.RaiseCombatHit(new CombatHit(attacker, gameObject, point, direction, damage, blocked, launched));
        }
    }
}
