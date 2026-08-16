using UnityEngine;
using UnityEngine.AI;
using SeasOfLegends.Combat;
using SeasOfLegends.Data;

namespace SeasOfLegends.AI
{
    /// <summary>
    /// Required components: NavMeshAgent, Animator, Combatant. Optional child Hitbox for melee.
    /// This compact hierarchical FSM keeps enemy decision-making separate from the player FSM.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(Combatant))]
    public sealed class EnemyController : MonoBehaviour
    {
        private enum State { Locomotion, Airborne, Attacking, Stunned, Blocking, Executing }

        [SerializeField] private Transform target;
        [SerializeField] private CombatSystem combatSystem;
        [SerializeField] private AttackDefinition basicAttack;
        [SerializeField] private float aggroDistance = 18f;
        [SerializeField] private float attackDistance = 2.2f;
        [SerializeField] private float blockChance = 0.12f;
        [SerializeField] private float stunSeconds = 0.35f;

        private NavMeshAgent agent;
        private Animator animator;
        private Hitbox hitbox;
        private State state;
        private float stateEndsAt;
        private bool hitboxArmed;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            hitbox = GetComponentInChildren<Hitbox>(true);
        }

        private void Update()
        {
            if (target == null || basicAttack == null) return;
            switch (state)
            {
                case State.Locomotion: UpdateLocomotion(); break;
                case State.Attacking: UpdateAttack(); break;
                case State.Blocking: if (Time.time >= stateEndsAt) SetState(State.Locomotion); break;
                case State.Stunned: if (Time.time >= stateEndsAt) SetState(State.Locomotion); break;
                case State.Executing: if (Time.time >= stateEndsAt) SetState(State.Locomotion); break;
            }
        }

        public void ReceiveHitStun(float duration)
        {
            stateEndsAt = Time.time + Mathf.Max(duration, stunSeconds);
            SetState(State.Stunned);
        }

        public void StartExecution(float duration)
        {
            stateEndsAt = Time.time + duration;
            SetState(State.Executing);
        }

        private void UpdateLocomotion()
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > aggroDistance) { agent.isStopped = true; return; }
            if (distance > attackDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                return;
            }
            agent.isStopped = true;
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up));
            if (Random.value < blockChance * Time.deltaTime) { stateEndsAt = Time.time + 0.3f; SetState(State.Blocking); }
            else SetState(State.Attacking);
        }

        private void UpdateAttack()
        {
            float elapsed = basicAttack.TotalSeconds - (stateEndsAt - Time.time);
            if (!hitboxArmed && elapsed >= basicAttack.StartupSeconds)
            {
                hitboxArmed = true;
                if (hitbox != null) hitbox.Arm(combatSystem, gameObject, basicAttack, 1);
            }
            if (hitboxArmed && elapsed >= basicAttack.StartupSeconds + basicAttack.ActiveSeconds)
            {
                hitboxArmed = false;
                if (hitbox != null) hitbox.Disarm();
            }
            if (Time.time >= stateEndsAt) SetState(State.Locomotion);
        }

        private void SetState(State next)
        {
            if (state == State.Attacking && hitbox != null) hitbox.Disarm();
            state = next;
            animator.SetBool("Blocking", state == State.Blocking);
            animator.SetBool("Stunned", state == State.Stunned);
            animator.SetBool("Executing", state == State.Executing);
            if (next == State.Attacking)
            {
                hitboxArmed = false;
                stateEndsAt = Time.time + basicAttack.TotalSeconds;
                animator.SetTrigger(basicAttack.AnimatorTrigger);
            }
        }
    }
}
