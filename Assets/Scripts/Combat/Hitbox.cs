using System.Collections.Generic;
using UnityEngine;
using SeasOfLegends.Data;

namespace SeasOfLegends.Combat
{
    /// <summary>
    /// Required components: trigger Collider. Place this on an animated weapon child and leave
    /// it disabled by default. CombatSystem controls its active window from frame data.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class Hitbox : MonoBehaviour
    {
        private readonly HashSet<Combatant> hitTargets = new HashSet<Combatant>();
        private Collider triggerCollider;
        private CombatSystem system;
        private GameObject owner;
        private AttackDefinition attack;
        private int comboCount;

        private void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
            triggerCollider.enabled = false;
        }

        public void Arm(CombatSystem combatSystem, GameObject attacker, AttackDefinition attackDefinition, int attackComboCount)
        {
            system = combatSystem;
            owner = attacker;
            attack = attackDefinition;
            comboCount = attackComboCount;
            hitTargets.Clear();
            triggerCollider.enabled = true;
        }

        public void Disarm()
        {
            triggerCollider.enabled = false;
            hitTargets.Clear();
            attack = null;
            owner = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (attack == null || owner == null) return;
            Combatant target = other.GetComponentInParent<Combatant>();
            if (target == null || target.gameObject == owner || !hitTargets.Add(target)) return;
            Vector3 point = other.ClosestPoint(transform.position);
            system.ResolveHit(owner, target, attack, point, (target.transform.position - owner.transform.position).normalized, comboCount);
        }
    }
}
